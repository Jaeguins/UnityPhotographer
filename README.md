# UnityPhotographer
유니티로 사진을 쉽게 찍는 개인소장용 라이브러리

대상과 하위오브젝트들을 사진촬영용 레이어로 할당하여 렌더링 후, 특정 색상을 뽑아 다른 색상으로 치환하여(대체로 배경을 투명화 할때 사용) 텍스쳐를 추출하여 이미지로 사용할 수 있게 합니다.

## 셋팅
- Color
  - chromaKey : 배경으로 바꿀 색
  - altColor : 배경색
- String
  - mask : 사진으로 찍힐 대상을 순간적으로 할당할 레이어의 이름
- GameObject
  - CameraObject : 카메라로 쓸 오브젝트, 안에 카메라 컴포넌트가 있어야 작동합니다.

## 사용법

`TakePicture(GameObject target, Vector3 camPosition, Vector3 aimPosition, bool isOrtho, float size=1f)`

- target : 대상 오브젝트
- camPosition : 대상 오브젝트로부터 어디로 카메라를 위치시킬지
- aimPosition : 대상오브젝트로부터 어디로 카메라를 조준할지
- isOrtho : 원근감 여부
- size : 원근감이 없을때의 축척
