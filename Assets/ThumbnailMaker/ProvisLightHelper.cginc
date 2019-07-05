#ifndef __CUSTOMINCLUDE_ProvisLightHelper_CGINC__
#define __CUSTOMINCLUDE_ProvisLightHelper_CGINC__

// https://answers.unity.com/questions/1268888/correct-way-of-calculating-tri-color-ambient-light.html
/*
half3 ShadeSHPerVertex (half3 normal, half3 ambient)
{
	#if UNITY_SAMPLE_FULL_SH_PER_PIXEL
		// Completely per-pixel
		// nothing to do here
	#elif (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
		// Completely per-vertex
		ambient += max(half3(0,0,0), ShadeSH9 (half4(normal, 1.0)));
	#else
		// L2 per-vertex, L0..L1 & gamma-correction per-pixel

		// NOTE: SH data is always in Linear AND calculation is split between vertex & pixel
		// Convert ambient to Linear and do final gamma-correction at the end (per-pixel)
		#ifdef UNITY_COLORSPACE_GAMMA
			ambient = GammaToLinearSpace (ambient);
		#endif
		ambient += SHEvalLinearL2 (half4(normal, 1.0));     // no max since this is only L2 contribution
	#endif

	return ambient;
}




// normal should be normalized, w=1.0
// output in active color space
half3 ShadeSH9 (half4 normal)
{
	// Linear + constant polynomial terms
	half3 res = SHEvalLinearL0L1 (normal);

	// Quadratic polynomials
	res += SHEvalLinearL2 (normal);

#   ifdef UNITY_COLORSPACE_GAMMA
		res = LinearToGammaSpace (res);
#   endif

	return res;
}




// normal should be normalized, w=1.0
half3 SHEvalLinearL0L1 (half4 normal)
{
	half3 x;

	// Linear (L1) + constant (L0) polynomial terms
	x.r = dot(unity_SHAr,normal);
	x.g = dot(unity_SHAg,normal);
	x.b = dot(unity_SHAb,normal);

	return x;
}
*/

/*
블렌딩 = SrcRGB * SrcAlpha + (1 - SrcAlpha) * DstRGB; (소스 알파 , 1 - 소스알파를 곱해서 얻는다.)
*/
inline fixed3 ComputeAlphaBlend(fixed3 src, fixed3 dst, fixed srcAlpha)
{
	return (src.rgb - dst.rgb) * srcAlpha + dst.rgb;
}

// 주변광 계산
inline fixed3 ComputeGradientAmbient(fixed3 normalWorld, fixed3 ambientGround, fixed3 ambientSky, fixed3 ambientEquator)
{
	//Magic constants used to tweak ambient to approximate pixel shader spherical harmonics 
	fixed3 worldUp = fixed3(0, 1, 0);
	float skyGroundDotMul = 2.5;
	float minEquatorMix = 0.5;
	float equatorColorBlur = 0.33;

	float upDot = dot(normalWorld, worldUp);

	//Fade between a flat lerp from sky to ground and a 3 way lerp based on how bright the equator light is.
	//This simulates how directional lights get blurred using spherical harmonics

	//Work out color from ground and sky, ignoring equator
	float adjustedDot = upDot * skyGroundDotMul;
	fixed3 skyGroundColor = lerp(ambientGround, ambientSky, saturate((adjustedDot * 0.5) + 1.0));

	//Work out equator lights brightness
	float equatorBright = saturate(dot(ambientEquator.rgb, ambientEquator.rgb));

	//Blur equator color with sky and ground colors based on how bright it is.
	fixed3 equatorBlurredColor = lerp(ambientEquator, saturate(ambientEquator + ambientGround + ambientSky), saturate(equatorBright * equatorColorBlur));

	//Work out 3 way lerp inc equator light
	float smoothDot = pow(abs(upDot), 2);
	fixed3 equatorColor = lerp(equatorBlurredColor, ambientGround, smoothDot) * step(upDot, 0) + lerp(equatorBlurredColor, ambientSky, smoothDot) * step(0, upDot);

	return lerp(skyGroundColor, equatorColor, saturate(equatorBright + minEquatorMix)) * 0.75;
}
#endif