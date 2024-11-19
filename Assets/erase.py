from PIL import Image as Im
import os, sys


folder = "Cozy Character v.2"


##############
def process_pic(path):
    image = Im.open(path)
    print(image.size)
    if image.size[0] > 256:
        image2 = image.crop((0, 0, 256, image.size[1]))
        print(image2.size)
        image2.save(path)


files = os.listdir(folder)

for f in files:
    path = os.path.join(folder, f)
    if os.path.isdir(path):
        pic_files = os.listdir(path)
        for pic_file in pic_files:
            if pic_file[-4:] != ".png":
                continue
            pic_path = os.path.join(path, pic_file);
            print(pic_path)
            process_pic(pic_path)

