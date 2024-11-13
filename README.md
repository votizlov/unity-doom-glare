# unity-doom-glare
Unity implementation of Doom 3's glare
Original inspiration: https://github.com/michaeldll/webgl-doom-glare and http://simonschreibt.de/gat/doom-3-volumetric-glow/  

[![Example](https://img.youtube.com/vi/x0iHw2pchz4/maxresdefault.jpg)](https://youtu.be/x0iHw2pchz4) 

Basic implementation of Doom 3's "Glare". It fakes a simple bloom effect by extruding vertices and interpolating their vertex colors.  
Works only in play mode.  
## How to use :  
- prepare a material with transparent shader that will use alpha channel from vertex color. Sample URP shadergraph included  
- assign GlarePlane on GameObject with meshFilter and meshRenderer
