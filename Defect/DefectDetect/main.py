import cv2 as cv
import numpy as np

from fastapi import FastAPI
import os

app = FastAPI()


@app.get("/{file_name}")
def read_root(file_name: str):
    img_rgb = cv.imread(file_name)
    assert img_rgb is not None, "file could not be read, check with os.path.exists()"

    img_gray = cv.cvtColor(img_rgb, cv.COLOR_BGR2GRAY)

    file_storage_path = "D:\Projects\DigitalDepartment\Defect\storage\defectData"
    result = []
    files = os.listdir(file_storage_path)
    for file in files:
        print(file)
        template = cv.imread(file_storage_path + '\\' + file, cv.IMREAD_GRAYSCALE)
        assert template is not None, "file could not be read, check with os.path.exists()"

        w, h = template.shape[::-1]
        res = cv.matchTemplate(img_gray,template,cv.TM_CCOEFF_NORMED)
        threshold = 0.8
        loc = np.where( res >= threshold)

        for pt in zip(*loc[::-1]):
            cv.rectangle(img_rgb, pt, (pt[0] + w, pt[1] + h), (0,0,255), 2)
            result.append([{"X": str(pt[0]), 'Y': str(pt[1])}, {'X': str(pt[0] + w), 'Y': str(pt[1] + h)}])
            
            print((pt[0] + w, pt[1] + h))
    #cv.rectangle(img_rgb, (302, 285), (336, 321), (0,0,255), 2)
    cv.imwrite('res.png',img_rgb)
    return { "Result": result }