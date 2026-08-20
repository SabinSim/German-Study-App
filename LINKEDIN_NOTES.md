# LinkedIn 포스팅용 메모 / LinkedIn Posting Notes

나중에 "build in public" 포스팅 작성할 때 참고할 소재들을 여기 모아둠.
Material to reference later when writing "build in public" posts. Kept in Korean and English side by side.

## EnsureCreated()의 한계 / The limits of EnsureCreated() (2026-08-20)

**무슨 일이 있었나 / What happened:**
- KR: `VocabEntry`에 `AgainCount`(리치 감지용 필드)를 새로 추가했더니, 앱 실행 시 `SQLite Error 1: no such column: v.AgainCount` 에러 발생.
- EN: After adding a new `AgainCount` field (for leech detection) to `VocabEntry`, the app crashed on launch with `SQLite Error 1: no such column: v.AgainCount`.

**원인 / Root cause:**
- KR: EF Core의 `EnsureCreated()`는 "DB 파일이 아예 없을 때만" 전체 스키마를 만들어줌. 이미 존재하는 테이블에 새 컬럼이 생겨도 자동으로 반영해주지 않음. 이번 세션에서 이미 한 번 겪었던 문제(그때는 테이블 자체가 없어서 "no such table: Decks" 에러)의 변형 버전 — 이번엔 테이블은 있는데 컬럼이 없는 경우.
- EN: EF Core's `EnsureCreated()` only builds the full schema when the database file doesn't exist yet. It never alters an already-existing table when the model changes. This was a variant of a problem I'd already hit earlier the same session ("no such table: Decks") — same root cause, but this time the table existed and only a column was missing.

**해결 (임시) / Fix (temporary):**
- KR: 로컬 DB 파일(`~/.germanstudyapp/germanstudyapp.db`)을 삭제하고 앱을 다시 실행 → `EnsureCreated()`가 처음부터 다시 만들면서 최신 스키마 전체 반영.
- EN: Deleted the local DB file (`~/.germanstudyapp/germanstudyapp.db`) and relaunched the app, so `EnsureCreated()` rebuilt the schema from scratch with the new column included.

**진짜 해결책 (앞으로 할 일) / The real fix (future work):**
- KR: 실사용자가 생기면 이 방식은 안 통함 (사용자 데이터를 지울 수 없으니까). EF Core Migrations로 전환해서, 스키마가 바뀔 때마다 "마이그레이션"이라는 변경 이력을 남기고 기존 DB에 점진적으로 반영하는 방식으로 가야 함.
- EN: This "just delete the DB" trick only works because there are no real users yet — you can't delete a real user's data. The proper fix is EF Core Migrations: each schema change gets recorded as a migration and applied incrementally to the existing database instead of rebuilding it.

**포스팅 각도 / Angles for the post:**
- KR: "빠르게 프로토타입 만들 때 쓰는 방법(EnsureCreated)과 실제 프로덕션에 필요한 방법(Migrations)의 차이"를 몸으로 배운 경험
- EN: A hands-on lesson in the difference between "fast prototyping" tooling (EnsureCreated) and what production actually requires (Migrations).
- KR: 스키마 변경 = 새 기능 추가할 때마다 반복되는 실수였고, 왜 이런 실수가 나는지 원리를 이해하게 됨
- EN: Every new feature that touched the schema reproduced the same bug — which forced me to actually understand *why* it kept happening, not just patch around it.
- KR: "왜 관계형 DB 마이그레이션이 백엔드에서 중요한 스킬인지" 체감한 사례로 쓰기 좋음
- EN: A concrete story for "why database migrations matter as a backend skill," grounded in a real bug instead of just reciting the concept.
