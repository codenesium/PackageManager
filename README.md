# PackageManager
This library creates minified zip files representing a directory structure.

The primary use case is deploying a zip file for a software deployment of some kind but anywhere a compact representation of a directory structure is needed would
be valid. 

When creating a package the library will iterate the directory tree of the passed directory and build an xml file manifest of 
the entire structure. This includes the folders and files. 

It then moves all unique files to a temp directory and zips all of the files along with the manifest.

When extracting it unzips the package and uses the manifest to recreate the directory structure and then populates that structure 
with the required files.

This way even if the same file is found in multiple places in the structure the file only exists in the package once.
This is accomplished by taking an MD5 hash of each file and only adding unique files to the package. 

