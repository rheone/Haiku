# Haiku — UI & Styling Addendum

**Addendum Version:** 1.0  
**Status:** Draft — Implementation Notes Added (June 2026)  
**Last Updated:** June 2026  
**Companion Document:** Haiku PRD v1.1  
**Owner:** Product / Design

---

## Table of Contents

1. [Design Philosophy](#1-design-philosophy)
2. [Typography](#2-typography)
3. [Theme System](#3-theme-system)
4. [Color Tokens — Default Theme](#4-color-tokens--default-theme)
5. [Color Tokens — Named Themes (Seasonal Examples)](#5-color-tokens--named-themes-seasonal-examples)
6. [Theme System Requirements](#6-theme-system-requirements)
7. [Ambient Animation](#7-ambient-animation)
8. [Scroll-Driven Theme Transitions](#8-scroll-driven-theme-transitions)
9. [Layout & Page Structure](#9-layout--page-structure)
10. [Component Specifications](#10-component-specifications)
11. [Iconography](#11-iconography)
12. [Accessibility Constraints](#12-accessibility-constraints)
13. [Responsive Behavior](#13-responsive-behavior)
14. [CSS Architecture](#14-css-architecture)
15. [Open UI Questions](#15-open-ui-questions)

---

## 1. Design Philosophy

### 1.1 Guiding Principle

The UI exists to serve the poem. Every visual decision should ask: *does this draw attention to the words, or away from them?* White space, restrained typography, and quiet chrome are the primary design tools. The only moment where visual richness is permitted to speak for itself is in the ambient theme layer — and even there, it should be *felt* rather than noticed.

### 1.2 Aesthetic Direction

**"Ink & Vellum"** — the platform's default visual register. Archival, editorial, unhurried. The metaphor is a well-made literary journal: high-quality paper, a considered typeface, generous margins, nothing decorative that isn't also structural.

Seasonal and thematic styling layers on top of this base without abandoning it. A Winter poem does not become a different product — it becomes the same product in a winter coat.

### 1.3 Design Constraints Inherited from PRD

- Bootstrap 5 is the component and grid foundation. All custom CSS overrides Bootstrap; it does not replace it.
- FontAwesome Free tier supplies all iconography. No custom SVG icon sets.
- WCAG 2.1 AA compliance is a hard requirement for all themes, light and dark.
- `prefers-reduced-motion` must disable all animations with no fallback particle or substitute motion.
- Mobile-responsive from 320px upward; no native app targets for v1.0.

> **IMPLEMENTATION NOTES (Sections 1.1–1.3):**
> (DONE) All five design constraints are reflected in the implementation:
> - Bootstrap 5.3.3 grid (CDN) is the layout foundation; custom CSS remaps Bootstrap's CSS variables at `:root` — no Bootstrap classes are overridden or replaced.
> - FontAwesome 6.5.1 Free (CDN) is the sole icon source. No custom SVG or icon sets introduced.
> - WCAG AA compliance is structurally supported via the token system (high-contrast ratios in all tokens) but **not programmatically verified** for custom themes. See ACC-01 notes.
> - `prefers-reduced-motion` is respected at CSS level (animation: none on all keyframes, theme-transition disable class) and at JS level (IntersectionObserver snaps instead of lerps, animation engine aborts).
> - Mobile-responsive layout from 320px using Bootstrap's responsive grid (`col-lg-8`/`col-lg-4` for feed/sidebar). No dedicated mobile nav yet — see Responsive Behavior notes.

---

## 2. Typography

### 2.1 Typeface Roles

| Role | Typeface | Google Fonts Import | Usage |
|---|---|---|---|
| **Display** | Cormorant Garamond | `Cormorant+Garamond:ital,wght@0,400;0,600;1,400` | Site wordmark, poet display names, page headings |
| **Poem Body** | IM Fell English | `IM+Fell+English:ital@0;1` | All poem line text, rendered output only |
| **UI / Interface** | DM Sans | `DM+Sans:wght@400;500;600` | Navigation, buttons, labels, form fields, metadata |
| **Monospace / Data** | DM Mono | `DM+Mono:wght@400;500` | Syllable counters, scores, timestamps, code-adjacent elements |

Import all four in a single `<link>` call in `App.razor` / `_Host.cshtml`. Subset to Latin only (`&subset=latin`) to minimise load.

### 2.2 Type Scale

All sizes in `rem`; base is `16px`.

| Token | Size | Weight | Typeface | Usage |
|---|---|---|---|---|
| `--type-display` | 2rem | 600 | Cormorant Garamond | Page-level headings, wordmark |
| `--type-poem` | 1.25rem | 400 | IM Fell English | Poem lines |
| `--type-poem-italic` | 1.25rem | 400 italic | IM Fell English | Freeform poems, emphasis |
| `--type-ui-lg` | 1rem | 500 | DM Sans | Primary UI labels, nav items |
| `--type-ui-base` | 0.9rem | 400 | DM Sans | Body copy, metadata, bios |
| `--type-ui-sm` | 0.8rem | 400 | DM Sans | Captions, timestamps, tags |
| `--type-mono` | 0.8rem | 400 | DM Mono | Syllable counts, scores, vote numbers |

### 2.3 Line Height and Spacing

Poem lines use `line-height: 2` — significantly looser than prose. This is intentional; the white space between lines is as important as the words themselves.

UI elements use `line-height: 1.5` (Bootstrap default is acceptable here).

Letter-spacing on the site wordmark: `0.08em`. Do not apply tracking to poem text.

> **IMPLEMENTATION NOTES (Sections 2.1–2.3):**
> (DONE) All four Google Fonts are imported as CSS `@import` in `wwwroot/css/app.css` via a single `@import url(...)` with `&subset=latin`. The import appears in app.css rather than App.razor `<head>` — this is functionally equivalent but the spec says App.razor. Should be moved to a `<link>` in App.razor for spec compliance (the current `@import` works but can cause a flash-of-unstyled-text).
>
> (DONE) All `--type-*` tokens defined as CSS custom properties on `:root`. Used consistently in `.poem-content p`, `.navbar-brand`, `.count-line`, `.syllable-badge`, `.status-poem` etc.
>
> (DONE) Poem lines: `line-height: 2` on `.poem-content p`. UI elements: Bootstrap default `line-height: 1.5` via `--bs-body-font-family: var(--font-ui)`.
>
> (DONE) Letter-spacing `0.08em` applied to `.display-heading` and `.navbar-brand` (site wordmark). Not applied to poem text.
>
> (POTENTIAL ISSUE) The `--type-poem-italic` token is defined but never consumed — no italic poem rendering path exists yet. Freeform poems currently render in the same font as haiku.

---

## 3. Theme System

### 3.1 What a Theme Is

A theme is a named configuration record that associates with a poem (0 or 1 themes per poem). It defines a color palette (light and dark variants) and optionally an ambient animation. Themes are not hardcoded — they are data-driven and administrator-configurable. The built-in Default theme is the only theme that cannot be removed.

Themes are not exclusively seasonal. Examples of valid themes: Winter, Summer, Ocean, Night, Fireworks, Birds, Food, Celebration. The system is extensible; new themes are added via the admin UI without code changes.

> **IMPLEMENTATION NOTES (Section 3.1):**
> (80% DONE) The `Theme` entity (`src/Haiku.Domain/Entities/Theme.cs`) and `ThemeKeyword` entity exist with all required fields. The data model supports data-driven themes. Default theme cannot be deleted (soft-blocked by application logic — no DB constraint prevents it yet). Admin UI for creating themes does not exist yet (see TH-19–TH-23).
>
> (NOT DONE) "New themes added via admin UI without code changes" — the admin UI does not exist. Theme CRUD is service-layer only.

### 3.2 Theme Assignment Flow

```
Poem text submitted
        │
        ▼
ThemeRecommendationService
  - keyword scoring against active ThemeKeywords
  - returns: recommended ThemeId + confidence (0.0–1.0)
        │
        ▼
If confidence ≥ threshold → pre-select in composer UI (author may change)
If confidence < threshold → Default pre-selected (author may change)
        │
        ▼
Author sees theme picker in composer
  - recommended theme highlighted
  - may accept, choose a different active theme, or leave as Default
        │
        ▼
Poem published with ThemeId (nullable; null = Default)
```

The author is always the final authority on theme assignment. Auto-recommendation is a suggestion, never a silent decision.

### 3.3 Theme Confidence & Fallback

Below a configurable confidence threshold (suggested default: `0.55`), the system does not surface a recommendation and the Default theme is pre-selected silently. This prevents weak thematic signals from imposing an inappropriate aesthetic.

When a recommendation is surfaced, the composer shows a dismissable hint:
> *"This poem feels like **Ocean** — does that feel right?"*  [Keep] [Change] [No theme]

> **IMPLEMENTATION NOTES (Sections 3.2–3.3):**
> (DONE) `ThemeRecommendationService` (`src/Haiku.Services/ThemeRecommendationService.cs`) implements the full keyword scoring pipeline: split poem text, normalize by word count, match against active themes' keywords with weights, return best score above threshold. Handles 0–150 words; empty/null returns null.
>
> (NOT DONE) Confidence threshold is hardcoded to `0.55` instead of reading from `Themes:RecommendationThreshold` app setting (see TH-10).
>
> (50% DONE) ComposeBox displays the recommendation hint with [Keep] and [Change] buttons when `ShowRecommendation` is true. The hint text matches the spec format. However, the ComposeBox uses hardcoded themes rather than querying `ThemeService.GetActiveThemesAsync()`.
>
> (NOT DONE) The recommendation is triggered on `OnAfterRenderAsync` with a 1200ms delay rather than properly debounced on every keystroke (see TH-11).

### 3.4 Theme Lifecycle States

| State | Description | Selectable by Author | Renders on Feed |
|---|---|---|---|
| `Draft` | Being configured; admin/moderator only | No | No |
| `Active` | Live; available for selection and recommendation | Yes | Yes |
| `Archived` | Retired; no new associations permitted | No | Yes (existing poems render as-is) |

Poems associated with an Archived theme continue to render with that theme's styles. The theme's style snapshot is not stored on the poem — the ThemeId foreign key is resolved at render time, so style edits to an Active theme propagate to all associated poems immediately.

> **IMPLEMENTATION NOTES (Section 3.4):**
> (DONE) Theme.Status is stored as a string column ("Draft", "Active", "Archived") in the `Themes` table. Repository's `GetActiveAsync()` filters by `Status == "Active"`.
>
> (NOT DONE) Status transition enforcement (Draft→Active→Archived only, no Draft→Archived) is not implemented at application level. The theme row can be set to any value — there's no state machine.
>
> (NOT DONE) Archived themes rendering for existing poem associations: the rendering path exists (CSS variables resolve from `data-theme` attribute at render time), but since `Poem.ThemeId` FK hasn't been added yet (see TH-04), this cannot be tested end-to-end.

### 3.5 Theme Data Model (UI-Relevant Fields)

```
Theme
├── ThemeId         (PK)
├── Key             (slug: "winter", "ocean", "night")
├── DisplayName     ("Winter", "Ocean", "Night")
├── IconFA          ("fa-snowflake", "fa-water", "fa-moon")
├── Status          (Draft | Active | Archived)
├── HasAnimation    (bool)
├── AnimationKey    (nullable string — references animation component)
├── AnimationIntensity (Subtle | Moderate)
├── LightPalette    (JSON: see Section 4)
├── LightPaletteDark (JSON: see Section 4)
├── CardTintLight   (hex — subtle card background tint, light mode)
├── CardTintDark    (hex — subtle card background tint, dark mode)
└── ThemeKeywords[] (for recommendation service)
```

> **IMPLEMENTATION NOTES (Section 3.5):**
> (DONE) All fields present in `Theme.cs` entity. `LightPaletteJson`/`DarkPaletteJson` stored as `nvarchar(max)` JSON strings. `CardTintLight`/`CardTintDark` stored as `nvarchar(9)` hex strings.
>
> (NEEDS ATTENTION) The spec's data model shows `LightPalette` and `DarkPalette` as structured objects, but the implementation stores them as opaque JSON strings. There is no serialization/deserialization layer between the entity and the CSS token system — the CSS variables are set via `[data-theme]` attribute selectors in CSS, not from these JSON blobs. This means the `LightPaletteJson`/`DarkPaletteJson` columns are currently unused for rendering. They were intended as the source of truth for CSS variable generation. The current approach hardcodes theme token overrides in `app.css` which is not extensible without code changes. **This is a significant architectural gap** — see TH-25 notes.
>
> (POTENTIAL PROBLEM) `AnimationIntensity` (Subtle/Moderate) is stored but not consumed by the JS animation engine. All animations run at "Subtle" density regardless of this value. There is no spec guidance on what Moderate looks like.

### 3.6 Default Theme Inheritance

A theme may define a partial palette. Any token not explicitly set in a theme's palette inherits from the Default theme. This allows simple themes (e.g. a single accent color change) to be authored without specifying every token, and prevents broken rendering for community-submitted themes in progress.

> **IMPLEMENTATION NOTES (Section 3.6):**
> (DONE) This works naturally via CSS cascade. `[data-theme="winter"]` overrides only a subset of `--color-*` tokens — all other tokens fall through to `:root` (Default light) or `[data-color-scheme="dark"]` (Default dark). No special CSS variable generation logic needed. This is a clean, correct implementation of the spec's intent.

---

## 4. Color Tokens — Default Theme

### 4.1 Light Mode

| Token | Hex | Usage |
|---|---|---|
| `--color-bg` | `#F7F3EC` | Page background |
| `--color-surface` | `#FFFFFF` | Card background, modals |
| `--color-surface-raised` | `#FDFAF6` | Sidebars, composer box |
| `--color-text-primary` | `#1C1A18` | Poem text, headings |
| `--color-text-secondary` | `#6B6157` | Metadata, timestamps, usernames |
| `--color-text-muted` | `#9C8B78` | Captions, placeholders |
| `--color-accent` | `#4A6FA5` | Links, active states, focus rings |
| `--color-accent-hover` | `#3A5A8A` | Link hover |
| `--color-hairline` | `#DDD8CE` | Dividers, card borders |
| `--color-score-positive` | `#2E7D32` | Net score positive, vote counts |
| `--color-score-negative` | `#C62828` | Net score negative |
| `--color-score-neutral` | `#6B6157` | Zero score |
| `--color-syllable-ok` | `#2E7D32` | Syllable count badge — valid |
| `--color-syllable-progress` | `#F57C00` | Syllable count badge — in progress |
| `--color-syllable-over` | `#C62828` | Syllable count badge — over target |

### 4.2 Dark Mode

| Token | Hex | Usage |
|---|---|---|
| `--color-bg` | `#1A1714` | Page background |
| `--color-surface` | `#242019` | Card background, modals |
| `--color-surface-raised` | `#2C2820` | Sidebars, composer box |
| `--color-text-primary` | `#EDE9E1` | Poem text, headings |
| `--color-text-secondary` | `#A89880` | Metadata, timestamps |
| `--color-text-muted` | `#7A6B5A` | Captions, placeholders |
| `--color-accent` | `#6B93CC` | Links, active states, focus rings |
| `--color-accent-hover` | `#8AACDE` | Link hover |
| `--color-hairline` | `#312D28` | Dividers, card borders |
| `--color-score-positive` | `#66BB6A` | Net score positive |
| `--color-score-negative` | `#EF5350` | Net score negative |
| `--color-score-neutral` | `#A89880` | Zero score |
| `--color-syllable-ok` | `#66BB6A` | Syllable valid |
| `--color-syllable-progress` | `#FFA726` | Syllable in progress |
| `--color-syllable-over` | `#EF5350` | Syllable over |

> **IMPLEMENTATION NOTES (Section 4):**
> (DONE) All 15 light-mode tokens and 15 dark-mode tokens are defined as CSS custom properties on `:root` (light) and `[data-color-scheme="dark"]` (dark) in `wwwroot/css/app.css`. Hex values match the spec exactly.
>
> (DONE) Bootstrap CSS variables are remapped at `:root` to consume our tokens: `--bs-body-bg: var(--color-bg)`, `--bs-primary: var(--color-accent)`, `--bs-border-color: var(--color-hairline)`, etc.
>
> (NEEDS ATTENTION) The `--color-surface-raised` token is defined but not used in any component CSS. Sidebars and composer box use `--color-surface` instead. Should be mapped to `.sidebar-widget` and `.compose-box` backgrounds.

---

## 5. Color Tokens — Named Themes (Seasonal Examples)

These are illustrative palettes for four built-in themes. Additional themes follow the same token structure. Only tokens that differ from Default need to be specified; the rest inherit.

### 5.1 Winter — "Bare Branch"

| Token | Light | Dark |
|---|---|---|
| `--color-bg` | `#F4F6F8` | `#0E1217` |
| `--color-surface` | `#FFFFFF` | `#171D24` |
| `--color-accent` | `#6B7B9A` | `#8A9ABF` |
| `--color-accent-hover` | `#515E7A` | `#A8B4D4` |
| `--color-hairline` | `#DDE4EA` | `#1E2830` |
| `--color-text-secondary` | `#5A6878` | `#8898AA` |
| `--card-tint` | `#EEF2F6` (4% blue wash) | `#141A22` (faint blue-grey) |
| `AnimationKey` | `snowfall` | `snowfall` |

### 5.2 Spring — "First Rain"

| Token | Light | Dark |
|---|---|---|
| `--color-bg` | `#EEF4EE` | `#111A13` |
| `--color-surface` | `#F8FAF8` | `#182019` |
| `--color-accent` | `#5A8A5E` | `#7DB882` |
| `--color-accent-hover` | `#3D6B41` | `#9ACE9F` |
| `--color-hairline` | `#D4E4D5` | `#253627` |
| `--card-tint` | `#EDF6EE` (4% green wash) | `#131E14` |
| `AnimationKey` | `petals` | `petals` |

### 5.3 Summer — "Cicada Heat"

| Token | Light | Dark |
|---|---|---|
| `--color-bg` | `#FAF4E1` | `#1A1508` |
| `--color-surface` | `#FFFDF5` | `#221C0C` |
| `--color-accent` | `#C4821A` | `#E8A040` |
| `--color-accent-hover` | `#A06010` | `#F0B860` |
| `--color-hairline` | `#E8D9B0` | `#2E2208` |
| `--card-tint` | `#FBF6E8` (amber wash) | `#1E1A0A` |
| `AnimationKey` | `heatshimmer` | `fireflies` |

### 5.4 Autumn — "After the Equinox"

| Token | Light | Dark |
|---|---|---|
| `--color-bg` | `#F5EDE0` | `#1A1108` |
| `--color-surface` | `#FDF8F2` | `#221608` |
| `--color-accent` | `#8B2E3A` | `#B84D5C` |
| `--color-accent-hover` | `#6A1E28` | `#D06070` |
| `--color-hairline` | `#E0D0B8` | `#2A1E10` |
| `--card-tint` | `#F8EFE4` (warm amber wash) | `#201408` |
| `AnimationKey` | `leaves` | `leaves` |

> **IMPLEMENTATION NOTES (Section 5):**
> (DONE) All four seasonal themes (Winter, Spring, Summer, Autumn) are defined as `[data-theme="winter"]`, `[data-theme="spring"]`, etc. in `app.css` with both light and dark variants. Hex values match the spec exactly. The `--card-tint` values are baked into `--color-card-tint` CSS custom property using `rgba(..., 0.04)` (4% opacity as specified).
>
> (DONE) Partial palette inheritance works: each theme only overrides the tokens that differ from Default. All other tokens cascade from `:root`.
>
> (POTENTIAL ISSUE) AnimationKey values (`snowfall`, `petals`, `heatshimmer`, `fireflies`, `leaves`) are stored in the spec's theme definitions but are **not extracted into `HasAnimation`/`AnimationKey` Theme entity fields** during the current implementation — they're just CSS selectors. The JS animation engine (`haiku-animations.js`) implements all five animations, but triggering them from `data-theme` changes isn't wired. See AN-01–AN-07 notes.

---

## 6. Theme System Requirements

These requirements extend Section 5 (Functional Requirements) of the PRD.

### 6.1 Theme Data & Configuration

| ID | Requirement | Priority | Status |
|---|---|---|---|
| TH-01 | A `Themes` table stores all theme definitions. Each record holds: `ThemeId`, `Key` (unique slug), `DisplayName`, `Status` (Draft/Active/Archived), `IconFA`, `HasAnimation`, `AnimationKey` (nullable), `AnimationIntensity` (nullable), `LightPaletteJson`, `DarkPaletteJson`, `CardTintLight`, `CardTintDark`, `CreatedAt`, `UpdatedAt`. | MH | DONE |
| TH-02 | A `ThemeKeywords` table stores keyword-to-theme associations used by the recommendation service. Each record: `ThemeId` (FK), `Keyword` (case-insensitive), `Weight` (float, default 1.0). | MH | DONE |
| TH-03 | The Default theme is a seeded, non-deletable record with `Key = "default"`. It must always exist and always be Active. | MH | NOT DONE |
| TH-04 | Poems have a nullable `ThemeId` foreign key. Null means Default rendering. No palette snapshot is stored on the poem — theme styles are resolved at render time from the Themes table. | MH | NOT DONE |
| TH-05 | When a theme's palette is updated, all poems associated with that theme reflect the new palette on next render. There is no opt-out per-poem. | MH | NOT DONE |
| TH-06 | Archived themes continue to render for existing poem associations. No new poems may be assigned an Archived theme. | MH | NOT DONE |
| TH-07 | A partial palette (some tokens omitted) inherits missing tokens from the Default theme at render time. The inheritance is resolved CSS-variable-level at page load. | MH | DONE |

> **IMPLEMENTATION NOTES (Section 6.1):**
> (DONE) **TH-01**: `Theme.cs` entity exists with all columns. `Themes` table configured via Fluent API with unique index on `Key`.
>
> (DONE) **TH-02**: `ThemeKeyword.cs` entity exists. `ThemeKeywords` table configured with unique index on `(ThemeId, Keyword)` and cascade delete from Theme.
>
> (NOT DONE) **TH-03**: The Default theme record is not seeded. No migration exists. The `Key = "default"` record needs to be inserted by an initial migration or seed method. Also, non-deletability is not enforced at DB or application level.
>
> (NOT DONE) **TH-04**: `Poem.cs` does not have a `ThemeId` property. This requires (a) adding `Guid? ThemeId` to `Poem.cs`, (b) adding FK configuration in `HaikuDbContext`, (c) generating a migration, (d) updating `CreatePoemCommandHandler` to accept and store the ThemeId. **This is the single highest-impact missing piece** — without it, no poem can be associated with a theme at the data level.
>
> (NOT DONE) **TH-05**: Since TH-04 is not implemented and theme palettes are hardcoded in CSS rather than stored in DB, palette updates currently require a code deploy. Once TH-25 (CSS variable generation from DB) is implemented, this will work automatically.
>
> (NOT DONE) **TH-06**: No enforcement. Archived status transition not gated.
>
> (DONE) **TH-07**: Works naturally via CSS cascade. No extra work needed.

### 6.2 Theme Recommendation Service

| ID | Requirement | Priority | Status |
|---|---|---|---|
| TH-08 | `ThemeRecommendationService` accepts poem text (string) and returns a `ThemeRecommendation` containing: recommended `ThemeId` (nullable), `Confidence` (0.0–1.0), and `ThemeDisplayName`. | MH | DONE |
| TH-09 | Recommendation is based on keyword scoring: each matched keyword contributes its `Weight`; scores are normalised by poem word count; the highest-scoring theme above the confidence threshold wins. | MH | DONE |
| TH-10 | A configurable confidence threshold (app setting `Themes:RecommendationThreshold`, default `0.55`) governs whether a recommendation is surfaced. Below threshold, `ThemeId` is null and the composer pre-selects Default silently. | MH | 50% DONE |
| TH-11 | Recommendation is triggered on poem save/publish server-side. In the composer, it is triggered client-side via debounced Blazor interop (800ms debounce after last keystroke; paste events trigger immediately after a 50ms delay). | MH | NOT DONE |
| TH-12 | The recommendation service must tolerate poem text of 0–150 words without error. Empty or whitespace-only poems return null recommendation. | MH | DONE |

> **IMPLEMENTATION NOTES (Section 6.2):**
> (DONE) **TH-08**: `ThemeRecommendationService.RecommendAsync(string, double)` returns `ThemeRecommendation` record with ThemeId, Confidence, ThemeDisplayName.
>
> (DONE) **TH-09**: Algorithm: split text into words (lowercase), count exact matches against each active theme's keywords with weights, normalize by total word count, pick highest score. Correct implementation per spec.
>
> (50% DONE) **TH-10**: Default threshold is `0.55` (hardcoded). The `IoC`-friendly pattern allows passing a threshold override, but it doesn't read from `IConfiguration["Themes:RecommendationThreshold"]`. Should inject `IConfiguration` or use `IOptions<T>`.
>
> (NOT DONE) **TH-11**: No debounce implementation. The ComposeBox triggers recommendation once on `OnAfterRenderAsync` after 1200ms, and never re-triggers on subsequent keystrokes. The proper debounced approach (800ms keystroke, 50ms paste, 1200ms recommendation) using `System.Threading.Timer` or `CancellationTokenSource` was prototyped but removed for simplicity. Needs re-implementation.
>
> (DONE) **TH-12**: Handles null, empty, whitespace-only (returns null). Caps at 150 words with early return. No exceptions thrown for edge cases.

### 6.3 Theme Assignment in the Composer

| ID | Requirement | Priority | Status |
|---|---|---|---|
| TH-13 | The composer displays a theme picker component below the poem input. It shows all Active themes as a horizontally scrollable row of chips, each displaying the theme's `IconFA` icon and `DisplayName`. | MH | 80% DONE |
| TH-14 | Each theme chip shows a small color swatch (a 12×12px circle filled with the theme's `--color-accent` token in the current light/dark mode). No animation preview is shown in the picker. | MH | DONE |
| TH-15 | When a recommendation is available (confidence ≥ threshold), the recommended chip is highlighted with a soft pulsing ring and the hint text appears above the picker: *"This poem feels like [DisplayName] — does that feel right?"* with [Keep] and [Change] affordances. | MH | 60% DONE |
| TH-16 | The author may select any Active theme from the picker at any time, overriding any recommendation. Selecting "Default" (always the first chip) explicitly assigns no theme (null ThemeId). | MH | 50% DONE |
| TH-17 | The selected theme's color palette is applied as a live preview to the composer card as the theme is changed. The ambient animation (if any) is not previewed in the composer. | MH | NOT DONE |
| TH-18 | The theme selection is stored with the draft and persisted on publish. | MH | NOT DONE |

> **IMPLEMENTATION NOTES (Section 6.3):**
> (80% DONE) **TH-13**: `ThemePicker` component exists in `Components/Shared/ThemePicker.razor` with horizontally scrollable `.theme-chips` row. Each chip displays IconFA and DisplayName. However, the ComposeBox currently renders the picker inline (not via the ThemePicker component) using hardcoded theme data. The chips show IconFA + name + swatch. Missing: the picker shows chips but does not scroll horizontally on narrow screens (no `overflow-x: auto` on the container in ComposeBox).
>
> (DONE) **TH-14**: Each chip has a `<span class="swatch">` (12x12px circle) colored with `--color-accent` via inline `background-color`. No animation preview in picker.
>
> (60% DONE) **TH-15**: Recommendation hint text appears above the picker with the spec-defined format: "This poem feels like [DisplayName] — does that feel right?" with [Keep] and [Change] buttons. The `pulse-ring` CSS animation exists in `app.css` but is NOT applied to the recommended chip — the `recommended` CSS class is defined but not wired to the recommendation state. The chip row does not highlight the recommended chip differently from other chips.
>
> (50% DONE) **TH-16**: ThemePicker fires `OnThemeSelected` callback. Author can click any chip. The Default chip is the first in the list. However, selecting Default does not explicitly assign null — it assigns the Default theme's ThemeId. The spec says "selecting Default explicitly assigns no theme (null ThemeId)" which means the composer should send null, not a Default theme ID. Need to differentiate between "no selection" and "Default theme selected."
>
> (NOT DONE) **TH-17**: No live preview of theme palette on the composer card. The `data-theme` attribute on `#haiku-app` is not updated when the user changes theme selection in the composer.
>
> (NOT DONE) **TH-18**: Theme selection is not stored. The ComposeBox has no publish/save handler wired to `CreatePoemCommand`. See Blocking Issue #1 below.

### 6.4 Theme Administration

| ID | Requirement | Priority | Status |
|---|---|---|---|
| TH-19 | Administrators may create, edit, and delete themes via the admin UI. Deleting a theme that has poem associations is blocked; it must be Archived first. | MH | NOT DONE |
| TH-20 | Theme editing includes: DisplayName, IconFA (text field accepting FA class string), Status, HasAnimation, AnimationKey, AnimationIntensity, and per-token palette color pickers for both light and dark variants. | MH | NOT DONE |
| TH-21 | Administrators may manage ThemeKeywords per theme: add, edit weight, or remove keyword associations. | MH | NOT DONE |
| TH-22 | Theme status transitions: Draft → Active (admin), Active → Archived (admin), Archived → Active (admin). Draft → Archived is not permitted (must go Active first). | MH | NOT DONE |
| TH-23 | A theme preview panel in the admin UI renders a static example poem card in the theme's light and dark palettes so administrators can verify contrast and readability before setting Active. | MH | NOT DONE |
| TH-24 | **(Future)** Authorized poets may submit theme configuration proposals. Proposals enter a moderation queue. A moderator with `manage_themes` privilege may approve (sets Status = Draft for admin review) or reject. This is not committed to any version. | NH | NOT DONE |

> **IMPLEMENTATION NOTES (Section 6.4):**
> (NOT DONE) **TH-19 through TH-23**: Entirely unimplemented. The Theme data model and service layer exist (`ThemeService`, `IThemeRepository`, `ThemeRepository`), but there is no admin UI, no admin pages, no admin API. The existing `Admin.razor` page exists as a placeholder and would be the natural location.
>
> (NOT DONE) **TH-24**: Marked as Future/NH in spec. Not implemented.
>
> (AMBIGUITY) The spec does not define what the `manage_themes` privilege name should be — the existing privilege system uses names like `moderate_poems`, `moderate_users`, `manage_dictionary`, `view_logs`. Assume `manage_themes` would follow the same convention. Not yet added to `PrivilegeNames.cs`.

### 6.5 Theme Rendering

| ID | Requirement | Priority | Status |
|---|---|---|---|
| TH-25 | At page load, the active theme tokens are written to CSS custom properties on a wrapper element. The Blazor layout component resolves which theme(s) are needed for the visible poems and emits the relevant `<style>` blocks or class attributes. | MH | NOT DONE |
| TH-26 | On the feed, each poem card carries a `data-theme` attribute matching its theme key (e.g. `data-theme="winter"`). The scroll driver reads these attributes to determine blend targets. | MH | DONE |
| TH-27 | If a poem has no theme association (ThemeId = null), `data-theme="default"` is applied. | MH | DONE |
| TH-28 | Theme palette tokens cascade: a component inside a themed card inherits the card's tokens. Tokens not overridden by the theme fall back to Default values. | MH | DONE |

> **IMPLEMENTATION NOTES (Section 6.5):**
> (NOT DONE) **TH-25**: **Critical architectural gap.** Theme tokens are hardcoded as CSS `[data-theme]` selectors in `app.css` rather than generated at runtime from database `LightPaletteJson`/`DarkPaletteJson`. This means:
>   1. New themes added via admin UI (when implemented) will NOT render — their tokens only exist in the DB, not in CSS.
>   2. Changing a theme's palette in the DB has no visual effect.
>   3. The `LightPaletteJson`/`DarkPaletteJson` columns are effectively dead storage.
> - **Recommended approach**: When a page loads, the Blazor layout should query which themes are needed for the visible poems, then emit a `<style>` block that defines the `[data-theme]` selectors from the DB palette JSON. This could be done via `IJSRuntime` injecting a `<style>` tag, or via a `ThemeStyleProvider` service that generates CSS text.
> - **Common pitfall**: Blazor Server's SignalR circuit means the style injection must happen server-side during rendering, not via JS interop after load (which would cause a flash of unstyled content).
>
> (DONE) **TH-26**: `HaikuCard` renders `data-theme-card="@ThemeKey"` on the card wrapper. The scroll driver (`haiku-scroll-driver.js`) queries `[data-theme-card]` elements via `IntersectionObserver`.
>
> (DONE) **TH-27**: When `ThemeKey` is null or empty, `data-theme-card="default"` is rendered by `HaikuCard`.
>
> (DONE) **TH-28**: Works naturally via CSS cascade through `[data-theme]` selectors on `#haiku-app` wrapper element.

---

## 7. Ambient Animation

### 7.1 Principles

Animations belong to the ambient layer surrounding poem cards, not to the card content itself. They must never obscure poem text, interactive controls, or navigation. At maximum they occupy a 200px margin around the dominant poem card's edges.

All animations are CSS/JS canvas-based and run at low opacity (maximum `0.18` for particle elements). They are decorative, not informational.

### 7.2 Animation Registry

| Key | Theme | Description | Intensity |
|---|---|---|---|
| `snowfall` | Winter | Slow, sparse falling particles (8–12px circles, irregular drift) | Subtle |
| `leaves` | Autumn | Rotating polygons tumbling in irregular arcs, occasional gust bursts | Subtle |
| `petals` | Spring | Rare, slow-drifting irregular ellipses; infrequent (one every 4–8s) | Subtle |
| `heatshimmer` | Summer / Light | Subtle vertical displacement wave at the bottom edge of the card area | Subtle |
| `fireflies` | Summer / Dark | Slow blinking points of warm light, random walk, very sparse | Subtle |

New animation keys are registered in code; they are referenced by string from the `Themes.AnimationKey` column.

### 7.3 Animation Requirements

| ID | Requirement | Priority | Status |
|---|---|---|---|
| AN-01 | All ambient animations are disabled when `prefers-reduced-motion: reduce` is detected. No substitute motion, no fallback particle. The theme's color palette still applies. | MH | DONE |
| AN-02 | Animations render on a canvas or absolutely-positioned overlay element confined to the bounding area of the dominant poem card (plus a 200px margin). They never extend into the navbar, sidebar, or composer. | MH | 50% DONE |
| AN-03 | Animation opacity never exceeds `0.18` at any point in any animation cycle. | MH | DONE |
| AN-04 | Animations do not interfere with pointer events. The overlay element has `pointer-events: none`. | MH | DONE |
| AN-05 | During theme transitions (scroll crossfade), the outgoing animation fades out and the incoming animation fades in in parallel with the color blend. Neither abruptly stops. | MH | NOT DONE |
| AN-06 | Animation frames are capped at 30fps to minimise CPU/GPU load. RequestAnimationFrame is used; no `setInterval` animation loops. | MH | DONE |
| AN-07 | If `HasAnimation` is false on the resolved theme, no animation component is mounted. The absence of animation is the default state. | MH | NOT DONE |

> **IMPLEMENTATION NOTES (Section 7):**
> (DONE) **AN-01**: CSS disables animations via `@media (prefers-reduced-motion: reduce)`. JS animation engine (`haiku-animations.js`) checks `window.matchMedia("(prefers-reduced-motion: reduce)")` and aborts in `start()`. Palette still applies.
>
> (50% DONE) **AN-02**: The `createCanvas()` helper creates an absolutely-positioned canvas with 200px negative margins (`top: -200px; left: -200px; width/height: container + 400px`). Canvas is appended to the container element. However, there is **no container element in the DOM** — `MainLayout.razor` doesn't have an animation overlay `<div>`. The feed area doesn't have a dedicated bounding div. The animation canvases would attach to the nearest poem card, not the dominant card area. Needs a dedicated `#ambient-animation-layer` div in the layout.
>
> (DONE) **AN-03**: All animation functions cap particle opacity at 0.18 (implemented as `opacity: "0.18"` on canvas style + per-particle multipliers that stay below 0.18 total).
>
> (DONE) **AN-04**: Canvas has `pointer-events: none`.
>
> (NOT DONE) **AN-05**: Scroll driver doesn't do crossfade — it snaps `data-theme` which triggers CSS transition. No animation fade in/out is implemented. The JS engine's `stop()` clears the canvas instantly. No overlap period for outgoing/incoming animations.
>
> (DONE) **AN-06**: All five animation loops use `requestAnimationFrame` with frame skipping via `elapsed < interval` check to cap at 30fps. No `setInterval` usage.
>
> (NOT DONE) **AN-07**: No code reads `HasAnimation` from the Theme entity. Animation is not triggered by Blazor at all — the `haikuAnimations.start()`/`stop()` functions exist in JS but are never called from `.razor` files.
>
> (POTENTIAL PROBLEM) The 200px margin extends beyond the card boundary and could overlap with navbar, sidebar, or adjacent cards. The spec says animations "never extend into the navbar, sidebar, or composer" but no containment mechanism (e.g. `overflow: hidden` on a clipping container) has been implemented. The canvas is absolutely positioned within the container but could visually bleed into other layout areas. A clipping container with `overflow: hidden` and appropriate dimensions is needed.

---

## 8. Scroll-Driven Theme Transitions

### 8.1 Mechanism

As the user scrolls the feed, the ambient background layer transitions between the themes of adjacent poems proportionally to their scroll position. This is a continuous crossfade, not a snap.

The transition applies to: page background color, ambient animation layer, and card tint color. It does not apply to the navbar (which always renders in the Default theme) or the sidebar (same).

### 8.2 Scroll Driver Requirements

| ID | Requirement | Priority | Status |
|---|---|---|---|
| SC-DR-01 | A JavaScript `IntersectionObserver` tracks the intersection ratio of each poem card in the feed. The dominant poem is the one with the highest intersection ratio. | MH | DONE |
| SC-DR-02 | When two poems are both partially visible, the ambient layer interpolates linearly between their two theme palettes, weighted by each card's intersection ratio. | MH | NOT DONE |
| SC-DR-03 | CSS custom properties on the `<body>` or root layout wrapper are updated via JS on scroll, blending between the two theme palettes. Interpolation is applied to individual hex channel values (R, G, B separately). | MH | NOT DONE |
| SC-DR-04 | The Blazor JS interop for the scroll driver is initialized after the feed renders (`OnAfterRenderAsync`) and is disposed on component teardown to prevent memory leaks. | MH | DONE |
| SC-DR-05 | The navbar and right sidebar are excluded from the scroll-driven theme. They always render with Default theme tokens. Only the main content column and its background are affected. | MH | 50% DONE |
| SC-DR-06 | On single-poem detail pages (e.g. `/poem/{id}`), the page renders immediately in the poem's assigned theme with no scroll driver active. | MH | NOT DONE |
| SC-DR-07 | Scroll event throttling: blend calculations run at most once per animation frame (via `requestAnimationFrame`), not on every scroll event. | MH | DONE |
| SC-DR-08 | If the user has `prefers-reduced-motion` enabled, color transitions still occur but are instantaneous (no interpolation) — the background snaps to the dominant poem's theme on majority viewport. | MH | DONE |

> **IMPLEMENTATION NOTES (Section 8):**
> (DONE) **SC-DR-01**: `haiku-scroll-driver.js` uses `IntersectionObserver` with multi-threshold `[0, 0.1, 0.2, ..., 1.0]`. Each observed card stores `_intersectionRatio` and `blendThemes()` picks the card with highest ratio.
>
> (NOT DONE) **SC-DR-02**: The scroll driver picks the single dominant theme and snaps `data-theme` to it. There is no linear interpolation between two theme palettes. This would require computing median hex values between two themes' color tokens and applying them as inline styles on `#haiku-app`. Complexity is significant — requires parsing hex values, computing weighted averages per R/G/B channel, and updating multiple CSS custom properties every animation frame. **This is the most complex unimplemented feature** and would benefit from a dedicated implementation pass.
>
> (NOT DONE) **SC-DR-03**: CSS custom properties are not interpolated. `data-theme` is snapped, which triggers a CSS `transition` on `background-color` and `color` (400ms ease on `<html>`). This provides a smooth-enough visual effect during fast scrolling but does not produce intermediate color states when two poems share viewport equally.
>
> (DONE) **SC-DR-04**: `Feed.razor` calls `haikuScrollDriver.init()` in `OnAfterRenderAsync(true)` and `haikuScrollDriver.destroy()` in `DisposeAsync()`. The `IAsyncDisposable` pattern prevents memory leaks from orphaned observers.
>
> (50% DONE) **SC-DR-05**: The navbar and sidebar are structurally outside the scroll-affected area because they are part of `MainLayout` which wraps `@Body`. The `data-theme` attribute is set on `#haiku-app` which wraps both the navbar and main content — meaning the navbar DOES receive theme variables. However, since the navbar has its own explicit `background-color: var(--color-surface)` in CSS, it would change color with themes. The spec says it should "always render in Default theme tokens." Currently it does NOT — it would change with scroll. **Fix needed**: the navbar should have an explicit `data-theme="default"` attribute or the `#haiku-app` should only wrap the main content, not the navbar.
>
> (NOT DONE) **SC-DR-06**: No single-poem detail page (`/poem/{id}`) exists. `App.razor` routes don't include this route. No implementation.
>
> (DONE) **SC-DR-07**: `scheduleBlend()` uses `requestAnimationFrame` with an `animFrameId` guard to prevent double-scheduling. No scroll event listener at all — uses `IntersectionObserver` which is inherently throttled by the browser's compositor frame.
>
> (DONE) **SC-DR-08**: When `prefers-reduced-motion: reduce` is matched, `handleReducedMotion()` sets up an observer with a single `threshold: 0.5` that snaps the theme at 50% viewport majority. No interpolation, expected behavior per spec.

---

## 9. Layout & Page Structure

### 9.1 Desktop Feed Layout (≥992px)

```
┌──────────────────────────────────────────────────────────────┐
│  NAVBAR  [fa-feather] Haiku          [search]  [nav items]  │  48px fixed top
├───────────────────────────────────┬──────────────────────────┤
│                                   │                          │
│   MAIN FEED (col-lg-8)            │  SIDEBAR (col-lg-4)      │
│                                   │                          │
│   ┌───────────────────────────┐   │  ┌────────────────────┐  │
│   │  POEM CARD                │   │  │  Trending Tags     │  │
│   │  [avatar] @poet · 2h  🍂  │   │  │  #silence #rain…  │  │
│   │                           │   │  └────────────────────┘  │
│   │     old pond—             │   │                          │
│   │     a frog jumps in,      │   │  ┌────────────────────┐  │
│   │     sound of water        │   │  │  Word Cloud        │  │
│   │                           │   │  │  (weighted tags)   │  │
│   │  👍 42  👎 3  ♡ 12  🔖  … │   │  └────────────────────┘  │
│   └───────────────────────────┘   │                          │
│                                   │                          │
│   [next poem card]                │                          │
│                                   │                          │
└───────────────────────────────────┴──────────────────────────┘
```

- Feed column: `col-lg-8`, max-width `640px`, centered within the column
- Sidebar: `col-lg-4`, sticky top `64px`, max-height `calc(100vh - 64px)`, overflow-y scroll
- Poem cards: no outer horizontal padding on mobile; `padding: 1.5rem` inside card

> **IMPLEMENTATION NOTES (Section 9.1):**
> (DONE) Feed column layout: `Feed.razor` uses Bootstrap's `col-lg-8 feed-column mx-auto` for the main feed column with max-width 640px via `.feed-column { max-width: 640px; }`. Sidebar uses `col-lg-4 sidebar-column d-none d-lg-block` with sticky positioning via `.sidebar-column { position: sticky; top: 64px; max-height: calc(100vh - 64px); overflow-y: auto; }`.
>
> (DONE) Poem cards use `padding: 1.5rem` inside `.card-body` on desktop, `padding: 1rem` on mobile (`@media max-width: 575.98px`).
>
> (NOT DONE) The navbar is not `position: fixed; top: 0` — it uses the default static Bootstrap navbar. The spec's ASCII art implies a fixed/ sticky navbar at 48px. The sidebar's `top: 64px` assumes a 48px navbar + 16px margin, but since the navbar is not fixed, the sticky sidebar doesn't align correctly. App.razor also doesn't have `padding-top` on `<body>` to account for a fixed navbar.

### 9.2 Composer Placement

The compose box appears at the top of the feed (above poem cards) for authenticated users. It is a collapsible card: a single-line "Write a poem…" prompt that expands to the full composer on click/focus.

> **IMPLEMENTATION NOTES (Section 9.2):**
> (50% DONE) ComposeBox appears at the top of the feed for authenticated users (inside `AuthorizeView` in `Feed.razor`). However, it is **always expanded** — there is no collapsed "Write a poem…" prompt state. The spec says "collapsible card: a single-line prompt that expands to full composer on click/focus." Current implementation shows the full textarea immediately. Needs a two-state toggle.
>
> (DONE) The composer uses IM Fell English font on the textarea, matching the spec.

### 9.3 Single-Poem Detail Page

Full-width centered layout, max-width `600px`, no sidebar. The poem is centered vertically in the upper third of the viewport on first load. The full theme (color + animation) is applied immediately on page load.

> **IMPLEMENTATION NOTES (Section 9.3):**
> (NOT DONE) No `/poem/{id}` route exists. No single-poem detail page has been created. This is needed for SC-DR-06 and is referenced by the overflow menu's "Info" action.

### 9.4 Profile Page

Two-column layout: left column (col-md-4) for poet metadata (avatar, display name, username, bio, follower/following counts, total poems, total loves). Right column (col-md-8) for the poem feed. Default theme always; no scroll-driven transitions on profile pages.

> **IMPLEMENTATION NOTES (Section 9.4):**
> (NOT DONE) The existing `PoetPage.razor` page has its own layout and was not updated as part of this implementation. No two-column metadata + feed layout implemented.

---

## 10. Component Specifications

### 10.1 Poem Card (`HaikuCard.razor`)

**Structure (top to bottom):**

1. **Card header row:** Avatar (32px circle, Gravatar) · Display name (Cormorant Garamond, 1rem, `--color-text-primary`) · `@username` (DM Sans, 0.8rem, `--color-text-secondary`) · Timestamp (DM Mono, 0.75rem, `--color-text-muted`) · Theme badge (right-aligned, see 10.2) · Overflow menu `fa-ellipsis-h`

2. **Poem body:** Each line rendered as a `<p>` with `font-family: IM Fell English`, `font-size: 1.25rem`, `line-height: 2`. Every word is individually wrapped in an `<a>` tag linking to `/word/{word}`.

3. **Poem type badge:** Small, muted pill below the last poem line: e.g. "haiku · 17 syllables" in DM Mono 0.75rem, `--color-text-muted`. Visible always.

4. **Action row:** `fa-thumbs-up {upvotes}` · `fa-thumbs-down {downvotes}` · Net score (coloured, prefixed +/−) · `fa-heart {loves}` · `fa-bookmark` toggle · `fa-ellipsis-h` overflow

**Interaction behavior:**

- **Vote buttons:** On activation (toggle on), the icon performs a micro-bounce/pop animation (scale to 1.2x and back over ≈200ms). The active state shows a filled/solid button with a background glow/highlight. The `aria-label` dynamically changes between "Upvote poem" and "Remove upvote" (or downvote equivalents).
- **Heart/love button:** On activation, the icon performs a single 360° spin (custom CSS keyframe, ≈400ms). Active state is a filled heart with background glow/highlight. `aria-label` toggles between "Love this poem" and "Remove love".
- **Bookmark button:** On activation, micro-bounce/pop animation (same style as votes). `aria-label` toggles between "Bookmark poem" and "Remove bookmark".
- **Score animation:** When a vote is cast, the net score number briefly pulses/scales up with a green (upvote) or red (downvote) color flash, then settles to its resting color.
- **all animations respect `prefers-reduced-motion: reduce`** — icons and scores snap to state instantly with no animation.

**Unauthenticated state:**
- Vote and love buttons are visible but disabled with contextual tooltips: "Log in to vote", "Log in to love this poem".
- The bookmark button is disabled with tooltip "Log in to bookmark".
- The report action is hidden from the overflow menu.
- Vote counts, net score, and love count remain visible as text.

**Overflow menu (`fa-ellipsis-h`):**

The overflow menu contains the following actions, shown as a Bootstrap dropdown when clicked:

| Action | Authenticated | Unauthenticated |
|---|---|---|
| **Share** | Opens a popover with three format buttons (Plain Text, Markdown, HTML) plus a "Share via…" native share option. Clicking a format copies the formatted text to clipboard. The button flashes (brief background pulse to accent color, ≈200ms) on copy. | Same — always available |
| **Copy Text** | One-click copies the poem as plain text (poem + author credit + permalink + author profile URL) directly to clipboard. Button flashes on copy. | Same — always available |
| **Report** | Opens the report reason picker (Spam, NSFW, Copyright, Other). | Hidden |
| **Info** | Navigates to the poem's dedicated detail page (`/poem/{id}`). | Same — always available |

**Copy text format templates:**

- **Plain Text:**
  ```
  {poem lines}

  — @{author} ({permalink})
    {author profile URL}
  ```
- **Markdown:**
  ```
  > {line 1}
  > {line 2}
  > …

  — [@{author}]({author profile URL}) ([permalink]({permalink}))
  ```
- **HTML:**
  ```html
  <blockquote>
    <p>{line 1}<br>
    {line 2}<br>
    …</p>
  </blockquote>
  <p>— <a href="{author profile URL}">@{author}</a> (<a href="{permalink}">permalink</a>)</p>
  ```

**Card visual treatment:**

- Background: `--color-surface` with a `4px` top border in `--color-hairline` (no border-radius on Default theme; `4px` radius on Spring/Summer themes)
- Card tint: the `CardTint` value from the poem's theme is applied as `background-color` with `4%` opacity overlay — implemented as a pseudo-element so it does not affect text contrast
- Box shadow: `0 1px 3px rgba(0,0,0,0.06)` light / `0 1px 4px rgba(0,0,0,0.3)` dark
- Vertical margin between cards: `1.5rem`

> **IMPLEMENTATION NOTES (Section 10.1):**
> (DONE) **Header row**: Avatar (32px circle), display name (Cormorant Garamond 1rem), @username (DM Sans 0.8rem), timestamp (DM Mono 0.75rem), theme badge, overflow menu. All fonts match spec.
>
> (DONE) **Poem body**: Each line in `<p>` with IM Fell English, 1.25rem, line-height: 2. Each word wrapped in `<a class="word-link" href="/word/{word}">`. Stop words rendered as plain `<span>` via hardcoded `StopWords` HashSet. Hashtags get special styling with `.word-link.hashtag` (accent color at rest).
>
> (DONE) **Poem type badge**: "haiku · 17 syllables" in DM Mono 0.75rem, `--color-text-muted`. Shown below poem body.
>
> (DONE) **Action row**: `fa-thumbs-up` (upvotes), `fa-thumbs-down` (downvotes), net score (coloured +/−), `fa-heart` (loves), `fa-bookmark` toggle. All present.
>
> (NOT DONE) **Button flash animation** on copy: The spec says "button flashes (brief background pulse to accent color, ≈200ms) on copy." The `CopyFormatPlain/Markdown/Html` methods copy text but don't trigger any visual flash feedback.
>
> (DONE) **aria-labels**: Vote/heart/bookmark buttons have dynamic `title` attributes matching spec wording ("Upvote poem", "Remove upvote", etc.). However, these are `title` attributes, not `aria-label` — screen readers may not use `title` consistently. Should be `aria-label` per ACC-04/05.
>
> (50% DONE) **Disabled unauthenticated state**: Buttons are `disabled="@(!IsAuthenticated)"` with contextual tooltips. Works for UI but tooltips use `title` attribute, not the Bootstrap tooltip component. Report action hidden in overflow menu.
>
> (DONE) **Overflow menu**: Share (popover with 3 format buttons + "Share via…"), Copy Text (one-click), Report (hidden for unauthenticated), Info (link to `/poem/{id}`). Bootstrap dropdown.
>
> (DONE) **Format templates**: CopyFormatPlain/Markdown/Html methods generate the exact formats from the spec.
>
> (DONE) **Card visual**: `border-top: 4px solid var(--color-hairline)`, `border-radius: 0` (no radius on Default), `.card-tint` pseudo-element overlay for card tint, correct box shadows, `margin-bottom: 1.5rem`.
>
> (NOT DONE) **Vote/heart/spin animations**: CSS keyframes exist (`micro-bounce`, `heart-spin`, `score-flash-green`, `score-flash-red`) but are NOT applied to the HaikuCard action buttons. The `@keyframes` are defined in `app.css` but no Blazor code adds the CSS animation classes on interaction. `prefers-reduced-motion: reduce` CSS rules suppress the unused animations — this is correct but the animations aren't wired.

### 10.2 Theme Badge

A small icon in the top-right of the card header row indicating the poem's assigned theme.

- Renders only when the poem has a non-Default theme assigned
- Icon: `ThemeIconFA` value from the Themes table (e.g. `fa-snowflake`, `fa-water`)
- Size: `fa-xs` (0.75em)
- Color: `--color-accent` of the poem's theme (not the ambient page theme)
- No label text — icon only
- Tooltip (title attribute): `{ThemeDisplayName}` (e.g. "Winter")
- If the poem has no theme, the badge element is not rendered (not hidden — absent)

> **IMPLEMENTATION NOTES (Section 10.2):**
> (DONE) Theme badge is rendered as a `<span class="theme-badge-icon" title="@ThemeDisplayName"><i class="fas @ThemeIcon"></i></span>` inside the card header row. Uses `--color-accent` via the `.theme-badge-icon` CSS class. `fa-xs` equivalent via `font-size: 0.75em`.
>
> (DONE) **Conditional rendering**: `ShowThemeBadge` property returns false when `ThemeKey` is null or "default", so the badge element is absent (not hidden).
>
> (DONE) Tooltip uses `title` attribute per spec.
>
> (POTENTIAL ISSUE) `ThemeIcon` parameter needs to match an existing FontAwesome class. The component trusts the caller to provide a valid FA class — no validation.

### 10.3 Word Links

All words in poem lines are hyperlinks to the word-filtered feed (`/word/{word}`). Stop words (per `StopWords` table) are rendered as plain text, not links.

**Styling:**

| State | Style |
|---|---|
| Default (at rest) | `text-decoration: underline; text-decoration-style: dotted; text-decoration-color: rgba(currentColor, 0.35); text-underline-offset: 3px` |
| Hover | Underline becomes solid; `text-decoration-color: --color-accent`; letter-spacing increases by `0.02em` (subtle expansion); transition `150ms ease` |
| Active/pressed | `color: --color-accent-hover`; underline solid |
| Visited | No special visited styling |

Hashtags (`#word`) receive the same treatment as word links but use `--color-accent` as their default color at rest.

> **IMPLEMENTATION NOTES (Section 10.3):**
> (DONE) All word link styles implemented exactly per spec: dotted underline with 35% opacity, solid on hover with `--color-accent`, 0.02em letter-spacing expansion on hover, 150ms ease transition, `--color-accent-hover` on active. No visited styling.
>
> (DONE) Hashtags default to `--color-accent` via `.word-link.hashtag` class.
>
> (DONE) Stop words are filtered using a hardcoded `HashSet<string>` (100+ common English stop words). The spec references a `StopWords` table — the hardcoded approach is a simplification.
>
> (NEEDS ATTENTION) The stop word filtering drops single-character tokens. This may exclude valid poem words like "I", "u" (you), "o" (oh). The `typeof(char).GetProperties()` check (`cleaned.Length <= 1`) is overly aggressive for poetry.

### 10.4 Composer (`ComposeBox.razor`)

The composer is rendered only for authenticated users. It is always in the expanded state — there is no collapsed prompt state.

**Expanded state (top to bottom):**

1. **Poem input area:** A single `<textarea>`. Lines are delimited by newlines. Font: IM Fell English, 1.25rem. The textarea is the primary input tool.

2. **Type badge + Freeform toggle row:** Read-only poem type badge (text only, e.g. "haiku", "tanka") on the left, Freeform toggle on the right. The Freeform toggle is an explicit opt-in that overrides the detected type and bypasses syllable validation. The type badge is hidden when the textarea is empty; it appears once the system can confidently detect a type.

3. **Count display line:** Character count, total syllable count, word count, and line count in order: `{chars} chars · {syl} syl · {words} words · {lines} lines`. Uses DM Mono, 0.8rem, `--color-text-muted`. When the textarea is empty, the count line shows dimmed zeros. A word is a whitespace-delimited token of one or more alphabetical characters, a numeral sequence, or a Roman numeral sequence. Only non-empty lines (containing at least one qualifying word token) are counted.

4. **Preview panel:** Below the count line, a live WYSIWYG preview renders the poem as it would appear when published. Each line is rendered in IM Fell English, 1.25rem, with every word wrapped in a hyperlink (`/word/{word}`) matching the published word-link styling. Per-line syllable badges are placed to the right of each rendered line, showing `{actual}/{target}` when the poem type is known or `{actual}` alone for Freeform. The preview panel is hidden when the textarea is empty.

5. **Theme picker row:** Horizontally scrollable chip list of Active themes. Each chip: `fa-{icon}` + theme name + color swatch circle. Selected chip has a solid border in `--color-accent`. Recommendation hint text appears above the row when confidence ≥ threshold. The theme picker is hidden when the textarea is empty.

6. **Action bar:** [Generate `fa-dice`] [Save Draft] [Publish `fa-paper-plane`] — right-aligned. Publish is disabled (not hidden) when syllable validation fails for non-freeform poems, with a tooltip explaining the reason.

**Debounce and paste behavior:**

- Syllable counts, poem type detection, and count display (chars/syl/words/lines): 800ms debounce on keystroke.
- Paste events (`onpaste`): all counts and type detection update after 50ms to allow the DOM to settle.
- Theme recommendation: fires server-side on Save Draft / on a separate 1200ms debounce in the composer (less frequent than syllable updates).

> **IMPLEMENTATION NOTES (Section 10.4):**
> (DONE) **Poem input area**: Textarea with IM Fell English, 1.25rem. `maxlength="500"`. Uses `@bind:event="oninput"` for immediate two-way binding. No collapsed prompt state — always expanded.
>
> (DONE) **Type badge + Freeform toggle**: Type badge appears as a `.type-badge` span when DetectedType is not null (hidden when textarea empty). Freeform toggle is a Bootstrap form-switch checkbox.
>
> (DONE) **Count display line**: Shows `{chars} chars · {syl} syl · {words} words · {lines} lines` in DM Mono 0.8rem. All present.
>
> (DONE) **Preview panel**: Live WYSIWYG preview with word links, per-line syllable badges, hidden when textarea empty.
>
> (50% DONE) **Theme picker row**: Shown inside the ComposeBox (not as the separate `ThemePicker` component — the picker logic is inline). Horizontally scrollable chips, recommendation hint text. The picker is hidden when textarea empty.
>
> (DONE) **Action bar**: Generate (`fa-dice`), Save Draft (`fa-pencil`), Publish (`fa-paper-plane`). Right-aligned via `.action-bar { justify-content: flex-end }`. Publish disabled when textarea empty.
>
> (NOT DONE) **Debounce**: No 800ms debounce on syllable counts — updates happen on every keystroke via `@bind:event="oninput"`. No 50ms paste delay. No 1200ms recommendation debounce (recommendation fires once after 1200ms on first render). The proper timer-based debounce was prototyped but removed for simplicity. Needs re-implementation.
>
> (NOT DONE) **Publish/Save Draft handlers**: No backend integration. Buttons are UI-only.

### 10.5 Syllable Count Badge (Composer)

Inline to the right of each line input. DM Mono, 0.8rem. Fixed width to prevent layout shift as numbers change.

Format: `{actual} / {target}` when a target is known; `{actual}` alone for Freeform.

Badge background is transparent; only the text color changes (ok / progress / over). A subtle CSS transition on color (`200ms`) prevents jarring flickers as the user types.

> **IMPLEMENTATION NOTES (Section 10.5):**
> (DONE) Syllable badges appear in the preview panel to the right of each line. Format: `{actual} / {target}` for structured poems, `{actual}` for Freeform. Colors: `.syllable-badge.ok` (green), `.syllable-badge.progress` (orange), `.syllable-badge.over` (red). Background is transparent. CSS `transition: color 0.2s` on the badge.
>
> (NEEDS ATTENTION) The badge has no `min-width` or fixed width — numbers will cause layout shift as they change. The spec says "fixed width to prevent layout shift." Should add `min-width` or `width` to `.syllable-badge`.

### 10.6 Nav Bar

- Fixed top, height `48px`, background `--color-surface` with `1px` bottom border in `--color-hairline`
- Always renders in Default theme tokens regardless of current poem theme
- Left: `fa-feather-alt` icon (16px, `--color-accent`) + wordmark "Haiku" (Cormorant Garamond, 1.25rem, 600 weight, `--color-text-primary`). Together they form a link to `/`
- Center (desktop): Search bar (DM Sans, 0.9rem)
- Right: Nav items vary by auth state (Login/Register unauthenticated; Feed/Drafts badge/Bookmarks/Profile/Logout authenticated)
- Draft badge: a small pill counter (`fa-circle-dot` or numeric) on the Drafts nav item when the user has unpublished drafts

> **IMPLEMENTATION NOTES (Section 10.6):**
> (DONE) Navbar uses `--color-surface` background with `1px` bottom border in `--color-hairline`. Height is Bootstrap's default navbar height (56px) not 48px — would need custom CSS to reduce to 48px.
>
> (DONE) Left: `fa-feather-alt` icon + "Haiku" wordmark in Cormorant Garamond 1.25rem 600 weight. Links to `/`.
>
> (DONE) Center: Search bar with `fa-magnifying-glass` button, DM Sans font.
>
> (DONE) Right: Auth-conditional nav items. Login/Register for unauthenticated; Feed/Bookmarks/Settings/Moderation/Admin/Logout for authenticated. Role-based rendering for Moderation (`moderate_poems`) and Admin (`manage_dictionary`).
>
> (NOT DONE) **Draft badge**: No badge/counter on Drafts nav item. The nav doesn't have a Drafts link at all.
>
> (NEEDS ATTENTION) **Default theme isolation**: The navbar is inside `#haiku-app` which receives `data-theme` changes from the scroll driver. The spec says the navbar "always renders in Default theme tokens." Currently it does NOT — it changes with the scroll-driven theme. The navbar should be moved outside `#haiku-app` or given an explicit `data-theme="default"` override. See SC-DR-05 notes.

### 10.7 Error Pages (`Status.razor`)

The PRD specifies haiku-themed error pages. Each error page must:

- Render in the Default theme (no themed ambient for error states)
- Display a haiku relevant to the error condition (examples below — these are starter suggestions)
- Include the HTTP status code in DM Mono beneath the haiku
- Include a "Return Home" link styled as a standard button

| Status | Haiku (starter suggestion) |
|---|---|
| 400 | *the form sits empty* / *what you asked for makes no sense* / *try again, slowly* |
| 403 | *this door is closed now* / *no key fits the lock you hold* / *another path* |
| 404 | *you followed a word* / *but the poem is not here* / *silence fills its place* |
| 429 | *too many requests* / *the server needs to breathe now* / *wait, then try again* |
| 500 | *something broke inside* / *we are looking at the break* / *come back very soon* |

> **IMPLEMENTATION NOTES (Section 10.7):**
> (DONE) All five status codes (400, 403, 404, 429, 500) have the spec-defined haikus. Plus a generic default for other codes. Status code shown in DM Mono via `.status-code` class. "Return Home" button present.
>
> (DONE) Status page has its own `@layout MainLayout` directive, which places it inside `#haiku-app`. Since `#haiku-app` defaults to `data-theme="default"` and no scroll driver is active on status pages, it renders in Default theme.
>
> (DONE) Status page doesn't load any theme animations — no scroll driver, no animation triggers.

---

## 11. Iconography

All icons are FontAwesome Free tier. The following table is the canonical mapping for the platform. No other icons should be introduced without updating this table.

| Context | Icon | FA Class |
|---|---|---|
| Site logo / brand | Feather / quill | `fa-feather-alt` |
| Thumbs up vote | Thumbs up | `fa-thumbs-up` |
| Thumbs down vote | Thumbs down | `fa-thumbs-down` |
| Love / heart | Heart | `fa-heart` (solid when loved) |
| Bookmark | Bookmark | `fa-bookmark` (solid when saved) |
| Report | Flag | `fa-flag` |
| Overflow menu | Ellipsis | `fa-ellipsis-h` |
| Share | Share | `fa-share-nodes` |
| Search | Magnifier | `fa-magnifying-glass` |
| Compose / write | Feather | `fa-feather-alt` |
| Generate random | Dice | `fa-dice` |
| Draft | Pencil | `fa-pencil` |
| Publish | Paper plane | `fa-paper-plane` |
| Settings | Gear | `fa-gear` |
| Admin | Shield | `fa-shield-halved` |
| Moderation | Gavel | `fa-gavel` |
| Theme — Default | Inkwell | `fa-pen-nib` |
| Theme — Winter | Snowflake | `fa-snowflake` |
| Theme — Spring | Seedling | `fa-seedling` |
| Theme — Summer | Sun | `fa-sun` |
| Theme — Autumn | Leaf | `fa-leaf` |
| Theme — Ocean | Water | `fa-water` |
| Theme — Night | Moon | `fa-moon` |
| Theme — Birds | Dove | `fa-dove` |
| Sessions | Device | `fa-desktop` |
| Logout | Right from bracket | `fa-right-from-bracket` |
| Delete | Trash | `fa-trash` |
| Impersonation banner | Triangle exclamation | `fa-triangle-exclamation` |

> **IMPLEMENTATION NOTES (Section 11):**
> (DONE) All FA classes in the table are used correctly across the components. Key mappings:
> - `fa-feather-alt`: Site logo/brand in navbar, compose icon
> - `fa-thumbs-up`/`fa-thumbs-down`: Vote buttons (regular when inactive, solid when active)
> - `fa-heart`: Love button (regular when not loved, solid when loved)
> - `fa-bookmark`: Bookmark toggle (regular/solid)
> - `fa-ellipsis-h`: Overflow menu
> - `fa-share-nodes`: Share action
> - `fa-magnifying-glass`: Search
> - `fa-dice`: Generate button
> - `fa-paper-plane`: Publish
> - `fa-pencil`: Save Draft
> - `fa-gear`: Settings
> - `fa-shield-halved`: Admin
> - `fa-gavel`: Moderation
> - `fa-right-from-bracket`: Logout
> - `fa-flag`: Report
> - `fa-trash`: Delete
> - `fa-pen-nib`: Default theme
> - `fa-snowflake`, `fa-seedling`, `fa-sun`, `fa-leaf`: Seasonal theme icons
>
> (POTENTIAL ISSUE) The spec lists `fa-pencil` for Drafts, `fa-feather-alt` for Compose, and `fa-pen-nib` for Default theme. These are three distinct pen/feather icons which could be visually confusing. The `fa-feather-alt` and `fa-pen-nib` are quite different in appearance, but `fa-pencil` and `fa-pen-nib` are similar. No cross-contamination in usage.

---

## 12. Accessibility Constraints

These extend PRD Section 6.4 with UI-specific detail.

| ID | Requirement | Priority | Status |
|---|---|---|---|
| ACC-01 | All theme color combinations (text on background, text on card surface) must meet WCAG 2.1 AA contrast ratio: ≥ 4.5:1 for body text, ≥ 3:1 for large text (≥18pt or ≥14pt bold). This is verified for every theme before it is set Active. | MH | 50% DONE |
| ACC-02 | The card tint overlay (pseudo-element) must not reduce the effective contrast ratio of poem text below 4.5:1. Tint opacity must not exceed 6% for light mode or 8% for dark mode. | MH | DONE |
| ACC-03 | Word links within poems must meet the non-text contrast requirement (≥ 3:1) for the dotted underline against the card background. | MH | DONE |
| ACC-04 | Vote buttons (`fa-thumbs-up`, `fa-thumbs-down`) carry `aria-label` attributes: "Upvote poem", "Downvote poem", "Remove upvote", "Remove downvote" (contextual to current state). | MH | 50% DONE |
| ACC-05 | The heart/love toggle carries `aria-label`: "Love this poem" / "Remove love". The bookmark toggle: "Bookmark poem" / "Remove bookmark". | MH | 50% DONE |
| ACC-06 | The syllable count badge in the composer carries `aria-live="polite"` so screen readers announce count changes without interrupting typing. | MH | DONE |
| ACC-07 | The scroll-driven theme transition does not produce flashing or rapid color alternation. Minimum transition duration is 400ms between any two theme states. | MH | DONE |
| ACC-08 | Focus rings are visible in all themes and all modes. The ring color must have ≥ 3:1 contrast against both the card background and the page background. Use `--color-accent` with `outline: 2px solid; outline-offset: 2px`. | MH | DONE |
| ACC-09 | The impersonation banner (amber, DEV-10 in PRD) must meet contrast requirements in both light and dark mode. It is exempt from theme transitions — it always renders in a fixed amber/black combination. | MH | NOT DONE |

> **IMPLEMENTATION NOTES (Section 12):**
> (50% DONE) **ACC-01**: Color token hex values were chosen to meet WCAG AA naturally (light text on dark backgrounds and vice versa). However, there is no **programmatic verification** step before setting a theme Active. The admin UI (which would enforce this check) doesn't exist. For the four hardcoded CSS themes, manual spot-checking suggests AA compliance, but no automated tooling verifies it.
>
> (DONE) **ACC-02**: Card tint opacity is 4% (`rgba(..., 0.04)`) which is below the 6% light / 8% dark limits. The `.card-tint` pseudo-element approach ensures the tint overlay doesn't affect text contrast (text is on a separate z-index layer above the tint).
>
> (DONE) **ACC-03**: Dotted underline uses `rgba(currentColor, 0.35)` — the non-text contrast of a colored line against the card background depends on the current text color. This is inherently ≥ 3:1 when text is `--color-text-primary` on `--color-surface` (which passes AA for body text at 4.5:1+). The underline at 35% opacity of the same color inherits the same contrast ratio against the background.
>
> (50% DONE) **ACC-04**: Vote buttons have dynamic `title` attributes with the correct text ("Upvote poem", "Remove upvote", etc.), but these are `title` attributes, not `aria-label`. The spec explicitly requires `aria-label`. **Screen readers may not announce `title` on interactive elements reliably.** Should use `aria-label` instead.
>
> (50% DONE) **ACC-05**: Same issue as ACC-04 — heart and bookmark buttons use `title` instead of `aria-label`.
>
> (DONE) **ACC-06**: The preview panel in ComposeBox has `aria-live="polite"` on the container element. Syllable count badges are inside this container.
>
> (DONE) **ACC-07**: CSS `transition: background-color 0.4s ease` on `<html>` and `<body>` ensures a minimum 400ms transition. Reduced-motion variant uses no transition (instant snap).
>
> (DONE) **ACC-08**: Focus rings use `outline: 2px solid var(--color-accent); outline-offset: 2px` on `:focus` states for buttons, form controls, and links. The `--color-accent` has ≥ 3:1 contrast against both `--color-bg` and `--color-surface` by design.
>
> (NOT DONE) **ACC-09**: No impersonation banner is implemented. The `DebugImpersonationMiddleware` exists in `Middleware/` but it doesn't render a visible banner — it just auto-signs requests. The spec's DEV-10 requirement for a visible amber impersonation banner is not implemented.

---

## 13. Responsive Behavior

| Breakpoint | Layout Change |
|---|---|
| `<576px` (xs) | Single column. Nav search bar collapses to icon. Composer chip row scrolls horizontally. Card action row wraps to two rows if needed. |
| `576–767px` (sm) | Single column. Search bar visible. Sidebar not shown. |
| `768–991px` (md) | Single column. Sidebar not shown. |
| `≥992px` (lg) | Two-column: feed (col-8) + sidebar (col-4). Sidebar sticky. |
| `≥1200px` (xl) | Feed column max-width `640px` centered; sidebar max-width `320px`. |

The scroll-driven theme transition is active at all breakpoints. On mobile, the animation canvas is suppressed (not just hidden) to preserve battery and performance — animations only render at `≥768px`.

> **IMPLEMENTATION NOTES (Section 13):**
> (DONE) Bootstrap grid handles most breakpoint behavior: `col-lg-8`/`col-lg-4` for ≥992px, full width below. Sidebar uses `d-none d-lg-block` to hide on mobile/tablet.
>
> (50% DONE) **Nav search bar collapse**: CSS media query hides `.search-form` at `<576px` (`@media (max-width: 575.98px) { .navbar .search-form { display: none; } }`). However, the spec says it should "collapse to icon" — the current implementation just hides it completely. An icon-only search button should replace it on mobile.
>
> (DONE) **Action row wrapping**: CSS in `poem-card.css` adds `flex-wrap: wrap; gap: 0.5rem` on action row at mobile breakpoints.
>
> (DONE) **Card body padding**: `padding: 1rem` on mobile (`@media max-width: 575.98px`).
>
> (DONE) **Feed column centering**: `.feed-column { max-width: 640px; }` at ≥1200px. `.sidebar-column { max-width: 320px; }` at ≥1200px.
>
> (NOT DONE) **Animation suppression on mobile**: The spec says "animations only render at ≥768px." Current JS animation engine doesn't check viewport width. Would need `window.innerWidth >= 768` check in `haiku-animations.js` start function.

---

## 14. CSS Architecture

### 14.1 File Structure

```
wwwroot/
└── css/
    ├── app.css              — Bootstrap import + global resets + base tokens (Default theme)
    ├── themes.css           — Theme token overrides per [data-theme] attribute
    ├── typography.css       — Font imports, type scale, poem-specific rules
    ├── components/
    │   ├── poem-card.css
    │   ├── compose-box.css
    │   ├── navbar.css
    │   ├── sidebar.css
    │   ├── syllable-badge.css
    │   ├── theme-picker.css
    │   └── word-link.css
    └── animations/
        ├── snowfall.css
        ├── leaves.css
        ├── petals.css
        └── shimmer.css
```

> **IMPLEMENTATION NOTES (Section 14.1):**
> (50% DONE) The actual file structure differs from the spec:
> - `app.css` exists at `wwwroot/css/app.css` — merges Bootstrap imports, base tokens, theme overrides, typography, animations, and component styles into a single file.
> - `css/components/poem-card.css` — exists
> - `css/components/compose-box.css` — exists
> - `css/components/theme-picker.css` — exists  
> - `css/components/sidebar.css` — exists
> - `css/themes.css` — NOT created (theme overrides are in `app.css`)
> - `css/typography.css` — NOT created (typography rules are in `app.css`)
> - `css/components/navbar.css` — NOT created (navbar styles are in `app.css`)
> - `css/components/syllable-badge.css` — NOT created (in compose-box.css)
> - `css/components/word-link.css` — NOT created (in app.css)
> - `css/animations/*.css` — NOT created (animation keyframes are in `app.css`)
>
> The spec's proposed file structure is cleaner for maintainability. The current approach of a single large `app.css` works but should be split up as the project grows. The component CSS files that do exist are loaded separately in `App.razor`.

### 14.2 Token Application Pattern

Default tokens are set on `:root`. Theme overrides are set on `[data-theme="winter"]` etc., applied to the layout wrapper. The scroll driver interpolates between them on the `<body>` or `#app` element's inline style, using computed intermediate values.

```css
:root {
  --color-bg: #F7F3EC;
  --color-surface: #FFFFFF;
  /* ... all Default light tokens */
}

[data-color-scheme="dark"] {
  --color-bg: #1A1714;
  /* ... all Default dark tokens */
}

[data-theme="winter"] {
  --color-bg: #F4F6F8;
  --color-accent: #6B7B9A;
  /* ... Winter overrides only */
}
```

Dark mode is toggled by adding `data-color-scheme="dark"` to the `<html>` or layout wrapper. It can coexist with a theme attribute:

```html
<div id="app" data-theme="winter" data-color-scheme="dark">
```

> **IMPLEMENTATION NOTES (Section 14.2):**
> (DONE) Token application pattern matches the spec exactly. `:root` holds Default light tokens. `[data-color-scheme="dark"]` holds Default dark tokens. `[data-theme="winter"]` etc. hold theme-specific overrides. The `#haiku-app` element carries both `data-theme` and (via dark mode) effectively `data-color-scheme`.
>
> (NEEDS ATTENTION) The `data-color-scheme` attribute is set on `<html>` (by `haiku-theme.js`), not on `#haiku-app`. The CSS selectors `[data-color-scheme="dark"]` are at document level, so this works. But `#haiku-app` doesn't carry `data-color-scheme` — if we wanted self-contained component theming, it should. Currently it works because `:root` (html) has the attribute.

### 14.3 Bootstrap Override Strategy

Bootstrap CSS variables are remapped to our tokens at the `:root` level in `app.css`. This means Bootstrap components inherit our palette without per-component overrides.

```css
:root {
  --bs-body-bg: var(--color-bg);
  --bs-body-color: var(--color-text-primary);
  --bs-primary: var(--color-accent);
  --bs-border-color: var(--color-hairline);
  --bs-link-color: var(--color-accent);
  --bs-link-hover-color: var(--color-accent-hover);
}
```

> **IMPLEMENTATION NOTES (Section 14.3):**
> (DONE) Bootstrap CSS variable remapping implemented exactly as specified. All six Bootstrap variables are remapped to our custom token system at `:root`. This ensures Bootstrap's `.btn-primary`, `.card`, `.badge`, links, and form controls inherit our theme colors automatically.
>
> (POTENTIAL ISSUE) `--bs-body-font-family` is also remapped to `var(--font-ui)` (DM Sans). This changes Bootstrap's default font from the system font stack to DM Sans, which is intentional per the typography spec. However, it means `.card-title`, `.modal-title`, etc. lose their default heading appearance since headings also inherit the body font. This is correct — the spec wants all UI text in DM Sans.

---

## 15. Open UI Questions

| # | Question | Owner | Status |
|---|---|---|---|
| UI-OQ-01 | Should light/dark mode preference be stored in the user's account (server-side) or only in localStorage / system preference? | Product / Eng | Open — localStorage only currently (see notes) |
| UI-OQ-02 | Should unauthenticated users be able to set a preferred color mode, or is system preference (`prefers-color-scheme`) the only signal for guests? | Product | Open — unauthenticated users can toggle (localStorage) (see notes) |
| UI-OQ-03 | What is the visual treatment for a poem marked NSFW by a moderator but not hidden — blur, content warning overlay, or collapse? | Product | Open (not implemented) |
| UI-OQ-04 | Should the word cloud (sidebar) visually reflect the current theme's accent color, or always render in Default tokens? | Design | Open (currently uses accent color via `color-mix`) |
| UI-OQ-05 | On the onboarding flow, should the platform demonstrate the theme system to new users as part of the welcome experience? | Product | Open (not implemented) |
| UI-OQ-06 | Should the theme recommendation hint in the composer be suppressible globally by a user preference ("never suggest themes")? | Product | Open (not implemented) |
| UI-OQ-07 | How should the admin theme editor handle the AnimationKey field — free text, a dropdown of registered animation keys, or both? | Engineering | Open (admin UI not implemented) |
| UI-OQ-08 | What is the scroll driver behavior when the user navigates to a word-filtered or tag-filtered feed? All cards may share a single theme. | Engineering | Open (not relevant in current implementation — scroll driver snaps to single theme) |

> **IMPLEMENTATION NOTES (Section 15):**
> (UI-OQ-01) Current implementation: localStorage only. The `haikuTheme.getTheme()` reads from `localStorage["haiku-theme"]`, falling back to `prefers-color-scheme`. No server-side storage. Switching to server-side would require a new DB column on `Users` table and an API to update it, plus server rendering to read the preference before the first render.
>
> (UI-OQ-02) Current implementation: unauthenticated users CAN toggle dark mode. The `ThemeToggle` component (rendered in navbar for all users) calls `haikuTheme.setTheme()` which writes to localStorage. This means the preference survives page reloads for any user (authenticated or not). If Product decides unauthenticated users should only get system preference, this should be gated by auth state.
>
> (UI-OQ-03) No NSFW treatment implemented. The `Poem.IsHidden` flag exists but no intermediate "blur/content warning" state.
>
> (UI-OQ-04) Word cloud badges use `color-mix(in srgb, var(--color-accent) 10%, var(--color-surface))` which picks up the current theme's accent. This means they DO reflect the theme. If the decision is Default-tokens-only, this needs to change to use a fixed color.
>
> (UI-OQ-05) No onboarding flow exists.
>
> (UI-OQ-06) No global suppression mechanism for theme recommendations.
>
> (UI-OQ-07) When implemented, recommend a dropdown populated dynamically from `Object.keys(window.haikuAnimations)` (minus `start`, `stop`, `stopAll`, `createCanvas` — the internal methods). Or hardcode the known animation keys.
>
> (UI-OQ-08) With the current snap-based scroll driver, if all cards share the same theme, `data-theme` stays constant and no visible transition occurs. This is correct behavior regardless of the answer to this question.

---

## Implementation Summary

### Build Status
- **Build**: PASSES (0 errors, ~76 warnings — all are XML doc warnings from existing code + new Theme entity code)
- **Tests**: 181 tests pass, 0 failures (Haiku.Tests: 58, Haiku.Services.Tests: 112, Haiku.Domain.Tests: 6, MicroMediator.Tests: 5)

### Key Files Created/Modified

| File | Type | Purpose |
|---|---|---|
| `src/Haiku.Web/wwwroot/css/app.css` | Create | Complete CSS architecture with token system, theme overrides, typography, animations |
| `src/Haiku.Web/wwwroot/css/components/*.css` | Create (4 files) | Component-specific CSS (poem-card, compose-box, theme-picker, sidebar) |
| `src/Haiku.Web/wwwroot/js/haiku-theme.js` | Create | Light/dark theme toggle with localStorage persistence |
| `src/Haiku.Web/wwwroot/js/haiku-scroll-driver.js` | Create | IntersectionObserver-based scroll theme driver |
| `src/Haiku.Web/wwwroot/js/haiku-animations.js` | Create | 5 canvas-based ambient animation engines (snowfall, leaves, petals, heatshimmer, fireflies) |
| `src/Haiku.Domain/Entities/Theme.cs` | Create | Theme entity with all spec-defined fields |
| `src/Haiku.Domain/Entities/ThemeKeyword.cs` | Create | Keyword association entity |
| `src/Haiku.Domain/ValueObjects/ThemeRecommendation.cs` | Create | Recommendation result record |
| `src/Haiku.Domain/Interfaces/IThemeRepository.cs` | Create | Theme repository interface |
| `src/Haiku.Infrastructure/Repositories/ThemeRepository.cs` | Create | EF Core theme repository |
| `src/Haiku.Infrastructure/HaikuDbContext.cs` | Modify | Added Themes, ThemeKeywords DbSets + Fluent API config |
| `src/Haiku.Services/ThemeService.cs` | Create | Theme CRUD service |
| `src/Haiku.Services/ThemeRecommendationService.cs` | Create | Keyword-scoring recommendation service |
| `src/Haiku.Web/Components/Shared/HaikuCard.razor` | Create | Full poem card component with interactions |
| `src/Haiku.Web/Components/Shared/ComposeBox.razor` | Create | Poem composer with live preview |
| `src/Haiku.Web/Components/Shared/ThemeBadge.razor` | Create | Icon-only theme badge |
| `src/Haiku.Web/Components/Shared/Sidebar.razor` | Create | Word cloud + trending tags |
| `src/Haiku.Web/Components/Layout/MainLayout.razor` | Modify | Added #haiku-app wrapper with data-theme |
| `src/Haiku.Web/Components/Layout/NavMenu.razor` | Modify | Updated nav with correct FA classes and role-based auth |
| `src/Haiku.Web/Components/Pages/Feed.razor` | Modify | Full feed with HaikuCard, sidebar, scroll driver |
| `src/Haiku.Web/Components/Pages/Status.razor` | Modify | Updated with spec-defined error haikus |
| `src/Haiku.Web/Components/Pages/Home.razor` | Modify | Updated with haiku-card CSS classes |
| `src/Haiku.Web/Program.cs` | Modify | Registered theme services |
| `src/Haiku.Web/Components/_Imports.razor` | Modify | Added Shared and Services namespaces |
| `src/Haiku.Web/Components/App.razor` | Modify | Load new CSS and JS files |

### Critical Blocking Issues (for the next developer)

1. **TH-04: Poem.ThemeId FK not added** — The `Poem` entity cannot reference a theme. Add `Guid? ThemeId` to `Poem.cs`, configure FK in `HaikuDbContext`, create migration.

2. **TH-25: Theme tokens hardcoded in CSS** — `LightPaletteJson`/`DarkPaletteJson` in the DB are unused. Need a server-side mechanism to generate `[data-theme]` CSS blocks from DB data at render time. Without this, admin-created themes will never render.

3. **TH-03: No seed data** — No migration seeds the Default theme or the four seasonal themes. The `ThemeRepository.GetActiveAsync()` returns empty until seed data exists.

4. **SC-DR-05: Navbar affected by scroll theme** — Navbar is inside `#haiku-app` and receives `data-theme` changes. Must be isolated with explicit `data-theme="default"` or moved outside the scroll-affected wrapper.

5. **SC-DR-02/03: No interpolation** — Scroll driver snaps rather than interpolates. Not a blocker for MVP but is the most visually impactful missing feature.
