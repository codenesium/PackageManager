# PackageManager
This package manager can be used to create a minified zip file representing a directory structure.

When create a package the library will iterate the directory tree of the passed directory and build an xml file manifest of 
the entire structure. This includes the folders and files. 

It then moves all unique files to a temp directory and zips all of the files along with the manifest.

When extracting it unzips the package and uses the manifest to recreate the directory structure and then populates that structure 
with the required files.

Doing is this way means that even if the same file is found in multiple places in the structure is only exists in the package once.
This is accomplished by taking an MD5 hash of each file and only adding unique files to the package. 
