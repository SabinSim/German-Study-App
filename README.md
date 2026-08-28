# German Study App

A desktop app for learning German that turns saved vocabulary into an Anki-style spaced-repetition trainer, with AI-powered sentence analysis on top. Built with C#/.NET and Avalonia UI, following a clean Core/Infrastructure/UI layered architecture.

This is both a personal study tool and a portfolio project. Every line in the `Core` and `Infrastructure` layers was written by hand (no scaffolding, no copy-pasted solutions) as a deliberate way to learn backend/.NET fundamentals from scratch. The UI layer was built with AI assistance to move faster, since the focus of this project is backend/domain logic.

## Features

**Sentence analysis**
- Paste any German sentence and get a breakdown of verb forms, separable verbs, article/gender reasoning, and adjective declension, powered by the OpenAI API.
- Toggle translation between English and Korean, and between natural and literal phrasing.
- Save unfamiliar words directly to a chosen deck.

**Vocabulary management**
- Nested decks (e.g. "German" containing "B1", "B2") with create/rename/delete and cycle-prevention.
- Vocabulary notebook grouped by the date words were added, with deck filtering and live search.
- Bulk import from a plain `.txt` file (`Word,Meaning` per line), with a preview step before saving.

**Flashcard review (Leitner system)**
- 4-level review grading (Again / Hard / Good / Easy) instead of simple right/wrong, closer to real spaced-repetition scheduling.
- Leech detection: words that keep getting marked wrong are flagged automatically.
- Leech-only review mode, card suspension, and undo for the last review answer.

**Study stats**
- Total words, total leeches, and a box-level distribution chart.
- Daily count of words saved, plus a 7-day forecast of upcoming review volume.

## Architecture

The solution is split into independently testable layers:

```
GermanStudyApp.Core            → interfaces and domain models only, no external dependencies
GermanStudyApp.Infrastructure  → implementations: OpenAI API client, EF Core + SQLite, Leitner algorithm
GermanStudyApp.UI              → Avalonia MVVM desktop app (CommunityToolkit.Mvvm)
GermanStudyApp.Tests           → xUnit unit tests for core scheduling logic
GermanStudyApp.ConsoleTest     → console harness for manually smoke-testing Core/Infrastructure
```

Core defines contracts such as `IGermanAnalysisService`, `IVocabRepository`, `IDeckRepository`, `IFlashcardService`, and `IVocabImportService`. Infrastructure provides the concrete implementations (`OpenAiAnalysisService`, `VocabRepository`/`DeckRepository` via EF Core/SQLite, `LeitnerFlashcardService`, `TxtVocabImportService`). The UI depends only on these interfaces, so the analysis engine, storage, or spaced-repetition logic can each be swapped or unit tested independently.

## Tech stack

- C# / .NET 10
- Avalonia UI 11 (cross-platform desktop, MVVM)
- CommunityToolkit.Mvvm (`ObservableProperty`, `RelayCommand`)
- Entity Framework Core + SQLite, with EF Core Migrations for schema changes
- xUnit for unit tests
- OpenAI API (grammar analysis and translation)
- GitHub Actions CI (build + test on every push)

## Getting started

**Prerequisites**
- .NET 10 SDK or later
- An OpenAI API key (only needed for sentence analysis)

**Setup**

```bash
export OPENAI_API_KEY=your-key-here
```

**Run the app**

```bash
dotnet run --project GermanStudyApp.UI
```

On first run, a local SQLite database is created at `~/.germanstudyapp/germanstudyapp.db`. Schema changes are applied automatically via EF Core Migrations, so existing data is preserved across updates.

**Run the tests**

```bash
dotnet test GermanStudyApp.Tests
```

**Build a standalone macOS app**

```bash
dotnet publish GermanStudyApp.UI -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true
```

## Project status

Actively in development. Core features (sentence analysis, deck management, vocabulary notebook, flashcard review, study stats, bulk import) are implemented and working end to end, backed by a CI pipeline and a growing unit test suite.

Next up: reworking sentence analysis so AI generates example sentences from saved vocabulary at a chosen difficulty level, then analyzes those generated sentences, rather than requiring manual sentence input.

## License

Personal project, currently unlicensed for reuse.
