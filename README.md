# German Study App

A desktop app for learning German that uses AI to analyze grammar in real sentences, then turns unfamiliar words into a spaced-repetition vocabulary trainer. Built with C#/.NET and Avalonia UI, following a clean Core/Infrastructure/UI layered architecture.

This is both a personal study tool and a portfolio project — every line in the `Core` and `Infrastructure` layers was written by hand (no scaffolding, no copy-pasted solutions) as a deliberate way to learn backend/.NET fundamentals from scratch.

## Features

- **AI-powered grammar analysis** — paste any German sentence and get a breakdown of verb forms, separable verbs, article/gender reasoning, and adjective declension, powered by the OpenAI API.
- **Translation with target language toggle** — switch between English and Korean, and toggle between a natural and a literal translation.
- **Vocabulary notebook** — select unfamiliar words with a checkbox and save them to a notebook, automatically grouped by the date they were added.
- **Flashcard review (Leitner system)** — saved words are reviewed on a spaced-repetition schedule (box levels 1–5); correct answers push a word further out, incorrect answers reset it.

## Architecture

The solution is split into independently testable layers:

```
GermanStudyApp.Core            → interfaces and domain models only, no external dependencies
GermanStudyApp.Infrastructure  → implementations: OpenAI API client, EF Core + SQLite, Leitner algorithm
GermanStudyApp.UI              → Avalonia MVVM desktop app (CommunityToolkit.Mvvm)
GermanStudyApp.ConsoleTest     → console harness for manually smoke-testing Core/Infrastructure
```

Core defines contracts such as `IGermanAnalysisService`, `IVocabRepository`, and `IFlashcardService`; Infrastructure provides the concrete implementations (`OpenAiAnalysisService`, `VocabRepository` via EF Core/SQLite, `LeitnerFlashcardService`). The UI depends only on these interfaces, so the analysis engine, storage, or spaced-repetition logic can each be swapped or unit tested independently.

## Tech stack

- C# / .NET 10 (Core and Infrastructure target-compatible with .NET 8)
- Avalonia UI 11 (cross-platform desktop, MVVM)
- CommunityToolkit.Mvvm (`ObservableProperty`, `RelayCommand`)
- Entity Framework Core + SQLite
- OpenAI API (grammar analysis and translation)

## Getting started

**Prerequisites**
- .NET 8 SDK or later
- An OpenAI API key

**Setup**

```bash
export OPENAI_API_KEY=your-key-here
```

**Run the app**

```bash
dotnet run --project GermanStudyApp.UI
```

On first run, a local SQLite database (`germanstudyapp.db`) is created automatically to store your vocabulary.

## Project status

Actively in development. Core features (sentence analysis, vocabulary notebook, flashcard review) are implemented and working end to end. Next up: CI pipeline (build + test on push) and packaged releases.

## License

Personal project, currently unlicensed for reuse.
