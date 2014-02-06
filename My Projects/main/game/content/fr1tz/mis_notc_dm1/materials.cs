// Copyright information can be found in the file named COPYING
// located in the root directory of this distribution.

singleton Material(fr1tz_mis_notc_dm1_mat0)
{
   footstepSoundId = 0;
   terrainMaterials = "1";
   ShowDust = "true";
   showFootprints = "true";
   effectColor[0] = "0.00784314 0.0313726 0.996078 1";
   effectColor[1] = "0.0313726 0.996078 0.00784314 1";
   glow[0] = "0";
   emissive[0] = "0";
   detailScale[0] = "64 64";
   scrollDir[0] = "0 -0.03";
   scrollSpeed[0] = "1.647";
   castShadows = "0";
   translucentBlendOp = "None";
   parallaxScale[0] = "1";
   useAnisotropic[0] = "1";
   diffuseColor[0] = "0.00392157 0.00392157 0.00392157 1";
   diffuseMap[1] = "content/fr1tz/mis_notc_dm1/textures/grid1.png";
   glow[1] = "1";
   emissive[1] = "1";
   detailScale[2] = "64 64";
   detailScale[1] = "64 64";
   diffuseColor[2] = "0.00392157 0.00392157 0.00392157 1";
   diffuseColor[1] = "0.992157 0.992157 0.992157 1";
};

singleton Material(fr1tz_mis_notc_dm1_termat1FX)
{
   mapTo = "fr1tz_mis_notc_dm1_termat1";
   footstepSoundId = 0;
   terrainMaterials = "1";
   ShowDust = "true";
   showFootprints = "true";
   materialTag0 = "Terrain";
   effectColor[0] = "0.00784314 0.0313726 0.996078 1";
   effectColor[1] = "0.0313726 0.996078 0.00784314 1";
   glow[0] = "0";
   emissive[0] = "0";
   detailScale[0] = "64 64";
   scrollDir[0] = "0 -0.03";
   scrollSpeed[0] = "1.647";
   castShadows = "0";
   translucentBlendOp = "None";
   parallaxScale[0] = "1";
   useAnisotropic[0] = "1";
   diffuseColor[0] = "0.00392157 0.00392157 0.00392157 1";
   diffuseMap[1] = "content/fr1tz/mis_notc_dm1/textures/grid1.png";
   glow[1] = "1";
   emissive[1] = "1";
   detailScale[2] = "64 64";
   detailScale[1] = "64 64";
   diffuseColor[2] = "0.00392157 0.00392157 0.00392157 1";
   diffuseColor[1] = "0.992157 0.992157 0.992157 1";
};

new TerrainMaterial()
{
   internalName = "fr1tz_mis_notc_dm1_termat1";
   diffuseSize = "16";
   detailSize = "4";
   detailDistance = "500";
   detailMap = "content/rotc/p.5.3/textures/rotc/zone.grid";
   diffuseMap = "content/fr1tz/mis_notc_dm1/textures/grey";
};
