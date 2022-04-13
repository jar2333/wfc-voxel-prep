# wfc-voxel-prep
Create json prototypes for voxels to be used in the Wave Function Collapse tiling algorithm.

In the WFC procedural generation algorithm (specifically its tiling variant), a set of tiles is defined, along with certain adjacency constraints.
One can define adjacency constraints by applying a label to the sides of each tile. Two tiles can be adjacent if their adjacent sides share the same label.
I call this label a "socket". 

This tool automatically generates sockets for a set of voxel meshes by keeping track of the vertices on each side of the voxels, and
assigning a unique socket to each unique side. The socket also specifies if the side is symmetric. 

Then, the tool saves a *prototype* of a WFC tile. This is a json file which contains the tile's mesh, its rotation, and its side's sockets. These prototypes
can then easily be loaded and used by a WFC algorithm implementation via a json deserializer.

References:
Martin Donald's WFC implementation: https://www.youtube.com/watch?v=2SuvO4Gi7uY
Marian Kleineberg's WFC implemetation: https://marian42.de/article/wfc/

To use (a sample scene is included which does steps 1-3):

1. Attach the CreateProtoypes script to an empty GameObject
2. Add cube/voxel meshes to the mesh array. Set the meshEdge float to be the distance between the center of the voxel its sides.
3. Add a prefab to use as the base for generating the prototype previews. The included _testPrefab_ has useful gizmos for visualizing the sockets.
4. Hit the "Create Sockets and then Previews" button to create sockets and then previews. A preview just shows the mesh with its new sockets in the Scene.
5. Hit the "Delete Previews and Reset Sockets" button to delete the already created sockets and previews. Useful for when one adds/deleted meshes.
6. Hit the "Create Prototypes" button to create the json prototypes from the meshes and pre-existing sockets.
