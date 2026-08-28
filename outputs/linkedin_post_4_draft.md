# LinkedIn Post #4 초안 / Draft

GitHub: https://github.com/SabinSim/German-Study-App

---

## 한국어

독일어 학습 앱 업데이트.

이번엔 Anki 써보면서 "아 이 기능 있으면 좋겠다" 싶었던 것들을 하나씩 실제로 만들어봤다. Again/Hard/Good/Easy 4단계 복습 평가, 계속 틀리는 단어 자동으로 표시해주는 리치(leech) 감지, 카드 일시정지, 리치만 골라서 복습하기, 단어장 검색, 앞으로 7일 복습량 미리 보여주는 통계, 복습 되돌리기(Undo), txt 파일로 단어 한번에 업로드까지.

근데 이번 업데이트에서 제일 기억에 남는 건 사실 새 기능이 아니라 버그였다. VocabEntry에 필드 하나 추가했다고 "no such column" 에러가 떴다. 원인 찾아보니 EF Core의 EnsureCreated()가 처음 DB 만들 때만 작동하고, 이미 있는 DB는 구조가 바뀌어도 그냥 무시해버리는 놈이었다. 그동안은 에러 날 때마다 로컬 DB를 그냥 지우고 다시 만드는 식으로 넘어갔는데(당연히 그때마다 저장해둔 단어 다 날아갔다), 이번엔 진짜 해결책인 EF Core Migrations로 갈아탔다. 스키마 바뀌어도 데이터는 그대로 유지된다. 왜 이게 필요한지 이번에 확실히 체감했다.

핵심 로직에 유닛 테스트도 처음으로 붙였다. 돌아보니 예전에 실제로 겪었던 버그들(박스 레벨이 최대치를 넘어버리던 것 같은)이 딱 이런 테스트로 바로 잡을 수 있는 종류였다.

마지막으로 dotnet publish로 macOS 앱(.app)까지 패키징해서, 이제 개발 도구 없이 더블클릭만으로 실행된다.

Core/Infrastructure는 계속 직접 코드 치면서 배우고, UI는 넘겨서 속도 내는 식으로 하고 있다. 지금까지는 이 조합이 꽤 괜찮다.

다음 계획: 문장 분석 기능을 뒤집어볼 생각이다. 지금은 사용자가 문장을 직접 입력하면 AI가 분석해주는 구조인데, 대신 저장해둔 단어를 갖고 AI가 예문을 만들어주고(레벨은 내가 선택), 그 예문을 다시 AI가 분석해주는 방식으로 바꿔보려고 한다. 단어 저장 → 실제 쓰이는 문맥까지 이어지는 흐름을 만들어보고 싶다.

#buildinpublic #dotnet #csharp #softwareengineering #sideproject

---

## English

German study app update.

Spent this round building out things I kept wishing for while using Anki: 4-level review grading (Again / Hard / Good / Easy), leech detection for words I keep getting wrong, card suspension, a leech-only review mode, vocab search, a 7-day forecast of upcoming reviews, undo for the last answer, and bulk import from a text file.

Honestly the part I remember most from this round wasn't a feature, it was a bug. Added one new field to VocabEntry and got a "no such column" error. Turned out EF Core's EnsureCreated() only builds the schema the first time the database is created, and just ignores any changes after that. My fix so far had been deleting the local database every time this happened (which meant losing whatever I'd saved). This time I actually switched to EF Core Migrations, so schema changes apply without wiping data. Small bug, but it made the "why do migrations matter" thing click in a way reading about it never did.

Also wrote unit tests for the core scheduling logic for the first time. Looking back, a couple of real bugs from earlier (like box levels going past the max) were exactly the kind of thing these tests would've caught right away.

And packaged a standalone macOS app with dotnet publish, so it runs with a double-click now, no dev tools needed.

Still hand-typing the Core/Infrastructure layer to actually understand it, delegating UI to move faster. Working out well so far.

What's next: flipping how sentence analysis works. Right now you type a sentence and AI analyzes it. Instead, AI will generate example sentences from words I've already saved (I pick the difficulty level), then analyze those generated sentences. Trying to build a flow that goes from saving a word to actually seeing it in context.

#buildinpublic #dotnet #csharp #softwareengineering #sideproject
