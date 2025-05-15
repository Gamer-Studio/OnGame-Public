# 🐙 파멸의 삼위일체 🐻 Trinity of Ruin 🐍
『파멸의 삼위일체』는 전설 속 세 괴수의 저주로 파괴된 세계를 되돌리기 위해, 플레이어가 각 스테이지의 보스와 맞서 싸우는 Top-down 로그라이크 슈팅게임입니다.
곰, 문어, 그리고 뱀으로 형상화된 세 저주를 정면으로 마주하고, 숨겨진 진실을 파헤치세요.

## 🧩 스토리
옛 전설에 따르면,
세 마리의 짐승 — 심연의 문어, 광기의 곰, 속삭이는 뱀 — 이 동시에 세상에 나타나는 순간, 세상은 파멸의 운명을 피할 수 없다고 했습니다.

사람들은 이 사건을 삼위일체의 저주라 불렀고, 오랜 시간 잊혀졌습니다.
하지만 이제, 그 짐승들이 다시 깨어났습니다.

플레이어는 저주받은 대륙을 순례하며,
각 보스를 무찌르고 그들의 힘을 봉인해야 합니다.
그리고 마지막엔... 세 저주의 근원이 드러납니다.


## 📖 목차
1. [프로젝트 소개](#프로젝트-소개)
2. [팀소개](#팀-소개)
3. [주요기능](#주요기능)
5. [개발기간](#개발기간)
6. [기술스택](#기술스택)
7. [와이어프레임](#와이어프레임)
8. [프로젝트 파일 구조](#프로젝트-파일-구조)
9. [Trouble Shooting](#trouble-shooting)
    
## 👨‍🏫 프로젝트 소개
내일배움캠프 9기 팀원들과 함께한 Top-down 로그라이크 슈팅게임입니다.

// 스토리 수정 예정

## 👥 팀 소개
팀장 김정인
팀원 방은성, 안상진, 박지환B, 고윤아


## 💜 주요기능

- 무기 강화 시스템

- 스테이지 내부 방 이동 기능

- 랜덤 방 생성

- 여러 공격 패턴을 가진 보스

- 랜덤 아이템 시스템

// 세부 설명 추가 예정


## ⏲️ 개발기간
- 2024.05.08(목) ~ 2024.05.14(수) (7일)

## 📚️ 기술스택

### ✔️ Language
- C#

### ✔️ Version Control
- Git
- GitHub

### ✔️ IDE
- Rider

### ✔️ Framework
- Unity 2022.3.17f


## 와이어프레임

![image](https://github.com/user-attachments/assets/6a8bbba6-a885-4a4d-acdb-07291d3f82d4)



## 프로젝트 파일 구조
// 수정 예정



## Trouble Shooting

# 플레이어가 NPC 상호작용 인식 범위 내에 들어와도 인식이 안 되는 현상

문제 요약
플레이어가 NPC 상호작용 인식 범위 내에 들어와도 인식이 안 되는 현상

재현 방법

NPC 인식 범위로 플레이어 이동
오류 확인
기대한 동작 (Expected behavior)
NPC 머리 위로 상호작용 UI가 뜨고, 그 상태에서 해당 상호작용 버튼을 누르면 이벤트가 실행되어야 함

Screenshots

추가 정보 (Additional context)
해당 동작은 InteractableObject.cs의 OnCollisionEnter2D 메서드 내부에서 충돌체의 Tag가 Player인지 확인 후 Gameobject.SetActive(true)를 통해 실행됨.

시도한 방법

NPC, Player 각각 알맞은 스크립트 붙어있는지 확인함
Player의 Tag - Player로 변경함
Debug.Log로 충돌 자체가 일어나고 있지 않음을 확인함
WorldCanvas의 Layer Foreground로 수정함

해결 방법
Player 자식 오브젝트인 Character에 rigidbody가 붙어있기 때문에 Player의 태그가 아닌 Character의 Tag를 Player로 바꾸어줘야 동작함.


#해상도 설정 UI에서 해상도 값이 표시 되지 않음

문제 요약
해상도 설정 UI에서 해상도 값이 표시 되지 않는 현상. 드래그와 선택은 가능함

재현 방법

메인 메뉴에서 Setting 버튼 클릭
해상도 설정 드롭다운 버튼 클릭
오류 확인
기대한 동작 (Expected behavior)
스크립트에서 연결한 해상도 설정 텍스트가 보이지 않음

Screenshots

![image](https://github.com/user-attachments/assets/38452a49-3415-4654-ab68-ddd62460f421)


추가 정보 (Additional context)
연결은 되어있으나 텍스트가 보이지 않는 상황으로 보입니다.


해결 방법
슬라이더 바가 TMP 지원을 안 해서 생긴 오류. 내부 TMP 컴포넌트를 전부 text로 교체 
