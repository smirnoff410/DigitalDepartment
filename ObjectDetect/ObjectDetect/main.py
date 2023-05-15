import cv2 as cv
import numpy as np
from fastapi import FastAPI

app = FastAPI()

#Скачать файл весов отсюда: https://pjreddie.com/media/files/yolov3.weights

#Изменить путь на свое локальное хранилище изображений
weights_path = "D:\\Projects\\DigitalDepartment\\ObjectDetect\\ObjectDetect"
storage_path = "D:\\Projects\\DigitalDepartment\\ObjectDetect\\storage"

@app.get("/{file_name}")
def read_root(file_name: str):
# load yolo
    net = cv.dnn.readNet(weights_path + "\\yolov3.weights",
                        weights_path + "\\yolov3.cfg")
    clasees = []
    with open("coco.names", 'r') as f:
        classes = [line.strip() for line in f.readlines()]
    # print(classes)
    layer_name = net.getLayerNames()
    output_layer = [layer_name[i - 1] for i in net.getUnconnectedOutLayers()]
    colors = np.random.uniform(0, 255, size=(len(classes), 3))

    # Load Image
    img = cv.imread(storage_path +"\\" + file_name)
    img = cv.resize(img, None, fx=0.4, fy=0.4)
    height, width, channel = img.shape

    # Detect Objects
    blob = cv.dnn.blobFromImage(
        img, 0.00392, (416, 416), (0, 0, 0), True, crop=False)
    net.setInput(blob)
    outs = net.forward(output_layer)
    # print(outs)

    # Showing Information on the screen
    class_ids = []
    confidences = []
    boxes = []
    for out in outs:
        for detection in out:
            scores = detection[5:]
            class_id = np.argmax(scores)
            confidence = scores[class_id]
            if confidence > 0.5:
                # Object detection
                center_x = int(detection[0] * width)
                center_y = int(detection[1] * height)
                w = int(detection[2] * width)
                h = int(detection[3] * height)
                # cv.circle(img, (center_x, center_y), 10, (0, 255, 0), 2 )
                # Reactangle Cordinate
                x = int(center_x - w/2)
                y = int(center_y - h/2)
                boxes.append([x, y, w, h])
                confidences.append(float(confidence))
                class_ids.append(class_id)

    # print(len(boxes))
    # number_object_detection = len(boxes)

    indexes = cv.dnn.NMSBoxes(boxes, confidences, 0.5, 0.4)
    print(indexes)

    result = []
    font = cv.FONT_HERSHEY_PLAIN
    for i in range(len(boxes)):
        if i in indexes:
            x, y, w, h = boxes[i]
            label = str(classes[class_ids[i]])
            # print(label)
            color = colors[i]
            cv.rectangle(img, (x, y), (x + w, y + h), color, 2)
            cv.putText(img, label, (x, y + 30), font, 3, color, 3)
            result.append({"Classification": label, "Coordinate" : [{"X": str(x), 'Y': str(y)}, {'X': str(x + w), 'Y': str(y + h)}]})

    #cv.imshow("IMG", img)
    #cv.waitKey(0)
    #cv.destroyAllWindows()
    #cv.imwrite('res.png',img)

    return { "Result": result, "Height": height, "Width": width }
