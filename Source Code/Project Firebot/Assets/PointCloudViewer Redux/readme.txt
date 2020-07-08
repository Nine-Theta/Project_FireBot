PointCloud Viewer Redux
Version 0.2.0.1

Originally adapted from the "Point Cloud Free Viewer" plugin by Gerard Llorach for Unity 4.3.2 (https://assetstore.unity.com/packages/tools/utilities/point-cloud-free-viewer-19811).
This package contains a Pointcloud ("xyzrgb_manuscript") from the Stanford Computer Graphics Laboratory which is not to be used for commercial purposes, nor should it appear in a product for sale (with the exception of scholarly journals or books), without their permission.

How to use this:
	To simply load a point cloud in a scene:
	Add the PointCloudManagerRedux script to a gameobject in the scene, it will automatically add the PointCloudBuilder script to the gameobject as well
	Then, drag the Pointcloud file into the TextAsset slot in the Editor, and select the appropriate file type.
	For PointCloudBuilder script, add the VertexColor Material to the slot in the Editor.
	This is enough for the scripts to work, and it will create a pointcloud at the position of the gameobject once run, but there are additional options:

	In PointCloudManagerRedux:
		Scale will modify the size of the pointcloud, by modifying the distance between points.	(default is 1)
		InvertYZ wil switch the Y and Z value of points, effectively rotating the pointcloud.	(default is false)

	In PointCloudBuilder:
		Mesh Point Limit determines the maximum amount of points in a single mesh. Larger values increase performance and load times. Lower values provide a smoother loading process.	(default is 65535)
		Enable Large Mesh Size allows "Mesh Point Limit" to go beyond the usual limit of 65535. The new limit is 4294967295. This is however not guaranteed to work on all hardware.	(default is false)
		Default Color determines the colour used for pointclouds that don't provide colour data in their file.	(default is green (0,255,0,255))


Script documentation:

--CustomTextAssetImporters--
	PointCloud data files aren't natively supported by the Unity Importer.
	This script is intended to be used for pointcloud data files which are stored in plain text, but use a different extension to identify themselves.
	Currently supported are: .off, .pts, and .xyz.
	formatting info:
		OFF:
			Line 0: <OFF identifier>
			Line 1: <number of verticies(points), number of faces, number of edges>
			Rest  : [Pointcloud data] 1 point per line, in XYZRGBA format
		
		PTS:
			Line 0: <amount of points>
			Rest  : [Pointcloud data] 1 point per line, in XYZARGB format

		XYZ:
			Rest  : [Pointcloud data] 1 point per line, in iXYZRGBA format (i(index) value is ignored)


--PointCloudManagerRedux--

Currently file types are somewhat hardcoded in the script.
Each file type is defined in an enum, which determines which IEnumerator file loader will be used.
These loaders are largely the same, but due to small inherent differences in the filetypes they are each their own IEnumerator.
They are IEnumerators so that they can be called using StartCoroutine(); which allows them to run across multiple Unity updates.
This in turn provides a smoother loading experience.

In general each loader works like this:
	Split the file to be loaded into indivdual lines, and store them in a string array.
	Create a string array to be used as a buffer for individual lines.
	Determine the amount of points in the file.
	Create a new vector3 array and a new color array, each the size of the amount of points.
	For each remaining line in the file, parse the coordinate data as a float and store it in the vector array (switching Y and Z if invertYZ is enabled).
	Parse the colour data if it's present, otherwise use the defaultColor.
	Update the GUI if a certain amount of points have been processed.
	Send the arrays, and a parent gameObject, to the PointCloudBuilder once all points have been processed.

To increase speed when parsing colours, a constant float, colDiv, is used.
As colours are usually stored in some form of RGB, which ranges from 0 to 255, it is necessary to divide them by 255 to convert them into Unity Colors, which are floats ranging from 0 to 1.
Division however is somewhat expensive, expecially for the amount of data pointclouds consist of, so the data is multiplied by a value instead. 
This value is 0.00392156862745098f, which is about equal to 1 divided by 255.
Due to the nature of floats, however, the actual value used is more around 0.003921569f.


--PointCloudBuilder--

The idea behind this script being separate from PointCloudManagerRedux was to allow it to be used by other file loaders.
As such it does not include any way of loading or saving files itself.

To start of, the method LoadPoints() is called by a script.
Several overload variants exist allow for multiple input cases.
It only requires point data, with options for a single color for everything, colors for every point is multiple formats, and a parent gameobject for the meshes.
Incase no color is provided, defaultColor will be used.
If no parent gameObject is provided, the script's gameObject will be used.
LoadPoints() then calls the corresponding IEnumerator Load().

As with PointCloudManagerRedux multiple IEnumarators exist, which generally work the same way.
This isn't ideal, however it circumvents the need to loop multiple time through each point.

In general each Load() works like this:
	Get the amount of points to be processed.
	Set the amount of groups the points will be distributed in.
	Create a new Vector3, integer, and color array, the length of the point limit of a group.
	For each group, assign the limit amount of points from the file to the group's arrays, and set an index for each point.
	Once the limit amount of points has been processed, create mesh out of it and instatiate the mesh as a gameobject.
	Update the GUI and continue with the next group.
	Once the groups are done, process one final group, which contains the leftover points.

These leftover points were not enough to fill the limit and were therefor not processed with the rest.
This is duplicate code, but circumvents the need for an if statement every group.

The InstantiateMesh function creates a new gameobject for the group, adds the neccesary components to it, and sets vertexMaterial as the render material.

CreateMesh, well, creates a new mesh out of the group's data.
If enableLargeMeshSize is set to true, UInt32 will be used as the format for the index counter, instead of the default UInt16.
In practice, this determines the maximum amount of points that can be displayed in a single mesh.
4294967295 for UInt32, and 65535 for the default UInt16.


--Current Issues--

- It was intended to save the finished pointcloud as a unity prefab, for more convenient later use, however this is non-functional in the current version.
- The GUI part of PointCloudManagerRedux also does not work right now.
- colDiv is a float, and as such cannot use the entire value it is given. Changing it to a double would work but likely increases processing time.
- PointCloudBuilder does not provide the option to return the mesh/finished pointcloud to the script that called it.
- PointCloudBuilder has a lot of duplicate code, which makes it somewhat difficult to work on. 