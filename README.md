# Master-s-thesis
Graph visualization tool in VR. Possibility to save and load hierarchical  and acyclic graphs. 

Layouting algorithms included: 
  * force-directed
  * temporal force-directed
  * cone tree
  * helix cone tree
  * cone burst (mine modification of the Cone Tree layout algorithm)
  * reconfigurable disk tree
  * default algorithm
  
It scans the 3D graph from each vertex point of an icosphere, and calculates the best perspective over the current graph layout by calculating quality metrics:
  * edge crossings
  * node overlapps
  * angle of edge crossings
  * angular resolution
  * edge length uniformity
  * total edge length
  * distinct angles
  * total graph area
