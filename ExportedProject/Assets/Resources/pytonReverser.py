import os
def loupe(paff):
    for filename in os.listdir(paff):
        if os.path.isdir(filename):
            loupe(filename)
        if filename.endswith("PYTONED.prefab"):
            print((paff + "/" + filename))
            nicename = (paff + "/" + filename).replace("PYTONED.prefab", ".prefab")
            os.rename((paff + "/" + filename),nicename)

loupe(os.getcwd())
