import os
def loupe(paff):
    for filename in os.listdir(paff):
        if os.path.isdir(filename):
            loupe(filename)
        if filename.endswith(".prefab.meta"):
            # mainObjectFileID: 0
            # mainObjectFileID: 100100000
            realpath = (paff + "/" + filename)
            culstring = ""
            f = open(realpath, "r")
            for x in f:
              culstring = culstring + x
            culstring = culstring.replace("mainObjectFileID: 0", "mainObjectFileID: 100100000")
            f.close()
            f = open(realpath, "w")
            f.write(culstring)

loupe(os.getcwd())
