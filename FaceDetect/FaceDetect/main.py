import time
from pathlib import Path
import numpy as np
import cv2 as cv
import torch

from emotion import detect_emotion, init

from models.experimental import attempt_load
from utils.datasets import LoadImages
from utils.general import check_img_size, non_max_suppression, \
    scale_coords
from utils.plots import plot_one_box
from utils.torch_utils import select_device, time_synchronized

from fastapi import FastAPI

app = FastAPI()

#Изменить путь на свое локальное хранилище изображений
storage_path = "D:\\Projects\\DigitalDepartment\\FaceDetect\\storage"

@app.get("/{file_name}")
def read_root(file_name: str):

    device = select_device('')
    init(device)
    # Load model
    model = attempt_load("weights/yolov7-tiny.pt", map_location=device)  # load FP32 model
    stride = int(model.stride.max())  # model stride
    imgsz = check_img_size(512, s=stride)  # check img_size

    # Set Dataloader
    dataset = LoadImages(storage_path + "\\" + file_name, img_size=imgsz, stride=stride)

    # Get names and colors
    names = model.module.names if hasattr(model, 'module') else model.names
    colors = ((0,52,255),(121,3,195),(176,34,118),(87,217,255),(69,199,79),(233,219,155),(203,139,77),(214,246,255))
    if device.type != 'cpu':
        model(torch.zeros(1, 3, imgsz, imgsz).to(device).type_as(next(model.parameters())))  # run once
    t0 = time.time()
    
    result = []
    for path, img, im0s, vid_cap in dataset:
        img = torch.from_numpy(img).to(device)
        img = img.float()  # uint8 to fp16/32
        img /= 255.0  # 0 - 255 to 0.0 - 1.0
        if img.ndimension() == 3:
            img = img.unsqueeze(0)

        # Inference
        t1 = time_synchronized()
        pred = model(img, augment=True)[0]

        # Apply NMS
        pred = non_max_suppression(pred, 0.5, 0.45, agnostic=True)
        t2 = time_synchronized()

        # Process detections
        for i, det in enumerate(pred):  # detections per image
            p, s, im0, frame = path, '', im0s.copy(), getattr(dataset, 'frame', 0)

            p = Path(p)  # to Path
            s += '%gx%g ' % img.shape[2:]  # print string
            gn = torch.tensor(im0.shape)[[1, 0, 1, 0]]  # normalization gain whwh
            if len(det):
                # Rescale boxes from img_size to im0 size
                det[:, :4] = scale_coords(img.shape[2:], det[:, :4], im0.shape).round()

                # Print results
                for c in det[:, -1].unique():
                    n = (det[:, -1] == c).sum()  # detections per class
                    s += f"{n} {names[int(c)]}{'s' * (n > 1)}, "  # add to string
                images = []

                for *xyxy, conf, cls in reversed(det):
                    x1, y1, x2, y2 = xyxy[0], xyxy[1], xyxy[2], xyxy[3]

                    images.append(im0.astype(np.uint8)[int(y1):int(y2), int(x1): int(x2)])
                
                if images:
                    emotions = detect_emotion(images)
                # Write results
                i = 0
                for *xyxy, conf, cls in reversed(det):
                    # Add bbox to image with emotions on 
                    label = emotions[i][0]
                    colour = colors[emotions[i][1]]
                    i += 1
                    plot_one_box(xyxy, im0, label=label, color=colour, line_thickness=2)
                    result.append({"Emotion": label, "Coordinate" : [{"X": int(xyxy[0]), 'Y': int(xyxy[1])}, {'X': int(xyxy[2]), 'Y': int(xyxy[3])}]})
            
            #cv.imshow("IMG", im0)
            #cv.waitKey(0)
            #cv.destroyAllWindows()
    
    return {"Result": result}


