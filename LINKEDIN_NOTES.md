# LinkedIn 포스팅용 메모 / LinkedIn Posting Notes

나중에 "build in public" 포스팅 작성할 때 참고할 소재들을 여기 모아둠.
Material to reference later when writing "build in public" posts. Kept in Korean and English side by side.

## 포스팅 #4 후보 소재: 포스트 #3 이후 변경사항 / Draft material for Post #4: everything since Post #3

포스트 #3(CI 파이프라인 관련 progress update)을 쓴 시점 이후로 진행된 작업들. 통계 기능(날짜별 학습 통계)까지 끝나면 이걸 합쳐서 포스트 #4로 정리하면 됨.
Everything done since Post #3 (the progress update mentioning the CI pipeline). Once the date-based study stats feature is done, fold it into this list for Post #4.

- KR: **GitHub Actions CI 파이프라인 실제로 구축 완료** — push할 때마다 자동으로 빌드 검증되도록 설정, 실패 사례도 여러 번 겪음.
  EN: **Actually built the GitHub Actions CI pipeline** — every push now triggers an automatic build check; ran into (and fixed) several real failures along the way.

- KR: **덱(Deck) 계층 구조 기능 전체 구현** — 생성/이름 수정/삭제, 단어 저장 시 덱 선택, 단어장 화면에서 덱별 필터링, 플래시카드 복습 시 덱 선택까지 4단계 전부. 부모-자식 관계(예: "German" 안에 "B1", "B2")를 지원하고, 순환 참조(자기 자신을 부모로 지정하는 등) 방지 로직도 넣음.
  EN: **Built the full Deck (nested folder) feature** — create/rename/delete, choosing a deck when saving words, filtering the vocabulary list by deck, and choosing a deck when reviewing flashcards. Supports parent-child nesting (e.g. "B1"/"B2" under "German") with cycle-prevention logic so a deck can't become its own ancestor.

- KR: **반복되는 CI 실패 패턴 발견 및 해결** — 새 파일을 만들 때마다 커밋에서 빠지는 실수를 여러 번 겪으면서, "로컬에서 되는 것"과 "저장소에 실제로 올라간 것"이 다를 수 있다는 걸 몸으로 배움.
  EN: **Found and fixed a recurring CI failure pattern** — new files kept getting left out of commits, which was a hands-on lesson that "works on my machine" and "what's actually in the repo" aren't the same thing.

- KR: **복습 평가를 2단계(맞음/틀림)에서 4단계(Again/Hard/Good/Easy)로 확장** — Anki 스타일 스케줄링에 가깝게 개선. `enum`, `switch`문 등 기본 문법도 이 과정에서 다시 다짐.
  EN: **Upgraded review grading from binary (right/wrong) to 4-level (Again/Hard/Good/Easy)**, closer to Anki-style scheduling. Also a good excuse to re-solidify fundamentals like `enum`s and `switch` statements.

- KR: **리치(Leech) 감지 기능 추가** — 계속 틀리는 단어를 자동으로 표시해주는 기능. 같은 세션 안에서 Again/Hard 단어가 다시 나오도록 큐 로직도 개선.
  EN: **Added leech detection** — automatically flags words that keep getting marked wrong. Also improved the review queue so Again/Hard cards resurface later in the same session instead of just disappearing.

- KR: **EF Core `EnsureCreated()`의 한계를 실전에서 겪음** — 아래 별도 항목 참고. 스키마 변경 시 기존 DB가 자동으로 안 바뀐다는 걸 두 번이나 직접 겪고 나서 이해함.
  EN: **Hit the limits of EF Core's `EnsureCreated()` in practice** — see the dedicated entry below. Learned the hard way (twice) that it never migrates an existing database when the model changes.

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
