#ifndef __CUSTOMINCLUDE_PROVISTRANSFORM_CGINC__
#define __CUSTOMINCLUDE_PROVISTRANSFORM_CGINC__

inline float4 Scaling(float4 vert, float x, float y, float z)
{
	float4x4 scaleMat;
	scaleMat[0][0] = 1.0 + x;
	scaleMat[0][1] = 0.0;
	scaleMat[0][2] = 0.0;
	scaleMat[0][3] = 0.0;
	scaleMat[1][0] = 0.0;
	scaleMat[1][1] = 1.0 + y;
	scaleMat[1][2] = 0.0;
	scaleMat[1][3] = 0.0;
	scaleMat[2][0] = 0.0;
	scaleMat[2][1] = 0.0;
	scaleMat[2][2] = 1.0 + z;
	scaleMat[2][3] = 0.0;
	scaleMat[3][0] = 0.0;
	scaleMat[3][1] = 0.0;
	scaleMat[3][2] = 0.0;
	scaleMat[3][3] = 1.0;

	return mul(scaleMat, vert);
}

#endif