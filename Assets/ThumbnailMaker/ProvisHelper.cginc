#ifndef __CUSTOMINCLUDE_ProvisHELPER_CGINC__
#define __CUSTOMINCLUDE_ProvisHELPER_CGINC__

// Condition이 1 이상이면 참, 역은 거짓입니다. 참일 경우 A를, 거짓일 경우 B를 리턴합니다.
#define PROVIS_BoolIf(A, B, Condition) (lerp(A, B, step(1, Condition)))

inline fixed3 LerpMobile(fixed3 a, fixed3 b, fixed c)
{
	return (a * (1 - c)) + (b * c);
}
#endif