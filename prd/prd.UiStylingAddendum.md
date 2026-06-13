# Haiku — UI & Styling Addendum

**Addendum Version:** 1.0  
**Status:** Draft  
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

---

## 3. Theme System

### 3.1 What a Theme Is

A theme is a named configuration record that associates with a poem (0 or 1 themes per poem). It defines a color palette (light and dark variants) and optionally an ambient animation. Themes are not hardcoded — they are data-driven and administrator-configurable. The built-in Default theme is the only theme that cannot be removed.

Themes are not exclusively seasonal. Examples of valid themes: Winter, Summer, Ocean, Night, Fireworks, Birds, Food, Celebration. The system is extensible; new themes are added via the admin UI without code changes.

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

### 3.4 Theme Lifecycle States

| State | Description | Selectable by Author | Renders on Feed |
|---|---|---|---|
| `Draft` | Being configured; admin/moderator only | No | No |
| `Active` | Live; available for selection and recommendation | Yes | Yes |
| `Archived` | Retired; no new associations permitted | No | Yes (existing poems render as-is) |

Poems associated with an Archived theme continue to render with that theme's styles. The theme's style snapshot is not stored on the poem — the ThemeId foreign key is resolved at render time, so style edits to an Active theme propagate to all associated poems immediately.

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

### 3.6 Default Theme Inheritance

A theme may define a partial palette. Any token not explicitly set in a theme's palette inherits from the Default theme. This allows simple themes (e.g. a single accent color change) to be authored without specifying every token, and prevents broken rendering for community-submitted themes in progress.

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

---

## 6. Theme System Requirements

These requirements extend Section 5 (Functional Requirements) of the PRD.

### 6.1 Theme Data & Configuration

| ID | Requirement | Priority |
|---|---|---|
| TH-01 | A `Themes` table stores all theme definitions. Each record holds: `ThemeId`, `Key` (unique slug), `DisplayName`, `Status` (Draft/Active/Archived), `IconFA`, `HasAnimation`, `AnimationKey` (nullable), `AnimationIntensity` (nullable), `LightPaletteJson`, `DarkPaletteJson`, `CardTintLight`, `CardTintDark`, `CreatedAt`, `UpdatedAt`. | MH |
| TH-02 | A `ThemeKeywords` table stores keyword-to-theme associations used by the recommendation service. Each record: `ThemeId` (FK), `Keyword` (case-insensitive), `Weight` (float, default 1.0). | MH |
| TH-03 | The Default theme is a seeded, non-deletable record with `Key = "default"`. It must always exist and always be Active. | MH |
| TH-04 | Poems have a nullable `ThemeId` foreign key. Null means Default rendering. No palette snapshot is stored on the poem — theme styles are resolved at render time from the Themes table. | MH |
| TH-05 | When a theme's palette is updated, all poems associated with that theme reflect the new palette on next render. There is no opt-out per-poem. | MH |
| TH-06 | Archived themes continue to render for existing poem associations. No new poems may be assigned an Archived theme. | MH |
| TH-07 | A partial palette (some tokens omitted) inherits missing tokens from the Default theme at render time. The inheritance is resolved CSS-variable-level at page load. | MH |

### 6.2 Theme Recommendation Service

| ID | Requirement | Priority |
|---|---|---|
| TH-08 | `ThemeRecommendationService` accepts poem text (string) and returns a `ThemeRecommendation` containing: recommended `ThemeId` (nullable), `Confidence` (0.0–1.0), and `ThemeDisplayName`. | MH |
| TH-09 | Recommendation is based on keyword scoring: each matched keyword contributes its `Weight`; scores are normalised by poem word count; the highest-scoring theme above the confidence threshold wins. | MH |
| TH-10 | A configurable confidence threshold (app setting `Themes:RecommendationThreshold`, default `0.55`) governs whether a recommendation is surfaced. Below threshold, `ThemeId` is null and the composer pre-selects Default silently. | MH |
| TH-11 | Recommendation is triggered on poem save/publish server-side. In the composer, it is triggered client-side via debounced Blazor interop (800ms debounce after last keystroke; paste events trigger immediately after a 50ms delay). | MH |
| TH-12 | The recommendation service must tolerate poem text of 0–150 words without error. Empty or whitespace-only poems return null recommendation. | MH |

### 6.3 Theme Assignment in the Composer

| ID | Requirement | Priority |
|---|---|---|
| TH-13 | The composer displays a theme picker component below the poem input. It shows all Active themes as a horizontally scrollable row of chips, each displaying the theme's `IconFA` icon and `DisplayName`. | MH |
| TH-14 | Each theme chip shows a small color swatch (a 12×12px circle filled with the theme's `--color-accent` token in the current light/dark mode). No animation preview is shown in the picker. | MH |
| TH-15 | When a recommendation is available (confidence ≥ threshold), the recommended chip is highlighted with a soft pulsing ring and the hint text appears above the picker: *"This poem feels like [DisplayName] — does that feel right?"* with [Keep] and [Change] affordances. | MH |
| TH-16 | The author may select any Active theme from the picker at any time, overriding any recommendation. Selecting "Default" (always the first chip) explicitly assigns no theme (null ThemeId). | MH |
| TH-17 | The selected theme's color palette is applied as a live preview to the composer card as the theme is changed. The ambient animation (if any) is not previewed in the composer. | MH |
| TH-18 | The theme selection is stored with the draft and persisted on publish. | MH |

### 6.4 Theme Administration

| ID | Requirement | Priority |
|---|---|---|
| TH-19 | Administrators may create, edit, and delete themes via the admin UI. Deleting a theme that has poem associations is blocked; it must be Archived first. | MH |
| TH-20 | Theme editing includes: DisplayName, IconFA (text field accepting FA class string), Status, HasAnimation, AnimationKey, AnimationIntensity, and per-token palette color pickers for both light and dark variants. | MH |
| TH-21 | Administrators may manage ThemeKeywords per theme: add, edit weight, or remove keyword associations. | MH |
| TH-22 | Theme status transitions: Draft → Active (admin), Active → Archived (admin), Archived → Active (admin). Draft → Archived is not permitted (must go Active first). | MH |
| TH-23 | A theme preview panel in the admin UI renders a static example poem card in the theme's light and dark palettes so administrators can verify contrast and readability before setting Active. | MH |
| TH-24 | **(Future)** Authorized poets may submit theme configuration proposals. Proposals enter a moderation queue. A moderator with `manage_themes` privilege may approve (sets Status = Draft for admin review) or reject. This is not committed to any version. | NH |

### 6.5 Theme Rendering

| ID | Requirement | Priority |
|---|---|---|
| TH-25 | At page load, the active theme tokens are written to CSS custom properties on a wrapper element. The Blazor layout component resolves which theme(s) are needed for the visible poems and emits the relevant `<style>` blocks or class attributes. | MH |
| TH-26 | On the feed, each poem card carries a `data-theme` attribute matching its theme key (e.g. `data-theme="winter"`). The scroll driver reads these attributes to determine blend targets. | MH |
| TH-27 | If a poem has no theme association (ThemeId = null), `data-theme="default"` is applied. | MH |
| TH-28 | Theme palette tokens cascade: a component inside a themed card inherits the card's tokens. Tokens not overridden by the theme fall back to Default values. | MH |

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

| ID | Requirement | Priority |
|---|---|---|
| AN-01 | All ambient animations are disabled when `prefers-reduced-motion: reduce` is detected. No substitute motion, no fallback particle. The theme's color palette still applies. | MH |
| AN-02 | Animations render on a canvas or absolutely-positioned overlay element confined to the bounding area of the dominant poem card (plus a 200px margin). They never extend into the navbar, sidebar, or composer. | MH |
| AN-03 | Animation opacity never exceeds `0.18` at any point in any animation cycle. | MH |
| AN-04 | Animations do not interfere with pointer events. The overlay element has `pointer-events: none`. | MH |
| AN-05 | During theme transitions (scroll crossfade), the outgoing animation fades out and the incoming animation fades in in parallel with the color blend. Neither abruptly stops. | MH |
| AN-06 | Animation frames are capped at 30fps to minimise CPU/GPU load. RequestAnimationFrame is used; no `setInterval` animation loops. | MH |
| AN-07 | If `HasAnimation` is false on the resolved theme, no animation component is mounted. The absence of animation is the default state. | MH |

---

## 8. Scroll-Driven Theme Transitions

### 8.1 Mechanism

As the user scrolls the feed, the ambient background layer transitions between the themes of adjacent poems proportionally to their scroll position. This is a continuous crossfade, not a snap.

The transition applies to: page background color, ambient animation layer, and card tint color. It does not apply to the navbar (which always renders in the Default theme) or the sidebar (same).

### 8.2 Scroll Driver Requirements

| ID | Requirement | Priority |
|---|---|---|
| SC-DR-01 | A JavaScript `IntersectionObserver` tracks the intersection ratio of each poem card in the feed. The dominant poem is the one with the highest intersection ratio. | MH |
| SC-DR-02 | When two poems are both partially visible, the ambient layer interpolates linearly between their two theme palettes, weighted by each card's intersection ratio. | MH |
| SC-DR-03 | CSS custom properties on the `<body>` or root layout wrapper are updated via JS on scroll, blending between the two theme palettes. Interpolation is applied to individual hex channel values (R, G, B separately). | MH |
| SC-DR-04 | The Blazor JS interop for the scroll driver is initialized after the feed renders (`OnAfterRenderAsync`) and is disposed on component teardown to prevent memory leaks. | MH |
| SC-DR-05 | The navbar and right sidebar are excluded from the scroll-driven theme. They always render with Default theme tokens. Only the main content column and its background are affected. | MH |
| SC-DR-06 | On single-poem detail pages (e.g. `/poem/{id}`), the page renders immediately in the poem's assigned theme with no scroll driver active. | MH |
| SC-DR-07 | Scroll event throttling: blend calculations run at most once per animation frame (via `requestAnimationFrame`), not on every scroll event. | MH |
| SC-DR-08 | If the user has `prefers-reduced-motion` enabled, color transitions still occur but are instantaneous (no interpolation) — the background snaps to the dominant poem's theme on majority viewport. | MH |

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

### 9.2 Composer Placement

The compose box appears at the top of the feed (above poem cards) for authenticated users. It is a collapsible card: a single-line "Write a poem…" prompt that expands to the full composer on click/focus.

### 9.3 Single-Poem Detail Page

Full-width centered layout, max-width `600px`, no sidebar. The poem is centered vertically in the upper third of the viewport on first load. The full theme (color + animation) is applied immediately on page load.

### 9.4 Profile Page

Two-column layout: left column (col-md-4) for poet metadata (avatar, display name, username, bio, follower/following counts, total poems, total loves). Right column (col-md-8) for the poem feed. Default theme always; no scroll-driven transitions on profile pages.

---

## 10. Component Specifications

### 10.1 Poem Card (`HaikuCard.razor`)

**Structure (top to bottom):**

1. **Card header row:** Avatar (32px circle, Gravatar) · Display name (Cormorant Garamond, 1rem, `--color-text-primary`) · `@username` (DM Sans, 0.8rem, `--color-text-secondary`) · Timestamp (DM Mono, 0.75rem, `--color-text-muted`) · Theme badge (right-aligned, see 10.2) · Overflow menu `fa-ellipsis-h`

2. **Poem body:** Each line rendered as a `<p>` with `font-family: IM Fell English`, `font-size: 1.25rem`, `line-height: 2`. Every word is individually wrapped in an `<a>` tag linking to `/word/{word}`.

3. **Poem type badge:** Small, muted pill below the last poem line: e.g. "haiku · 17 syllables" in DM Mono 0.75rem, `--color-text-muted`. Visible always.

4. **Action row:** `👍 {upvotes}` · `👎 {downvotes}` · Net score (coloured, prefixed +/−) · `♡ {loves}` · `🔖` bookmark toggle · `fa-flag` report · `fa-ellipsis-h` overflow

**Interaction behavior:**

- **Vote buttons:** On activation (toggle on), the icon performs a micro-bounce/pop animation (scale to 1.2x and back over ≈200ms). The active state shows a filled/solid button with a background glow/highlight. The `aria-label` dynamically changes between "Upvote poem" and "Remove upvote" (or downvote equivalents).
- **Heart/love button:** On activation, the icon performs a single 360° spin (custom CSS keyframe, ≈400ms). Active state is a filled heart with background glow/highlight. `aria-label` toggles between "Love this poem" and "Remove love".
- **Bookmark button:** On activation, micro-bounce/pop animation (same style as votes). `aria-label` toggles between "Bookmark poem" and "Remove bookmark".
- **Score animation:** When a vote is cast, the net score number briefly pulses/scales up with a green (upvote) or red (downvote) color flash, then settles to its resting color.
- **all animations respect `prefers-reduced-motion: reduce`** — icons and scores snap to state instantly with no animation.

**Unauthenticated state:**
- Vote and love buttons are visible but disabled with contextual tooltips: "Log in to vote", "Log in to love this poem".
- The bookmark button is disabled with tooltip "Log in to bookmark".
- The report (`fa-flag`) button is hidden entirely.
- Vote counts, net score, and love count remain visible as text.

**Card visual treatment:**

- Background: `--color-surface` with a `4px` top border in `--color-hairline` (no border-radius on Default theme; `4px` radius on Spring/Summer themes)
- Card tint: the `CardTint` value from the poem's theme is applied as `background-color` with `4%` opacity overlay — implemented as a pseudo-element so it does not affect text contrast
- Box shadow: `0 1px 3px rgba(0,0,0,0.06)` light / `0 1px 4px rgba(0,0,0,0.3)` dark
- Vertical margin between cards: `1.5rem`

### 10.2 Theme Badge

A small icon in the top-right of the card header row indicating the poem's assigned theme.

- Renders only when the poem has a non-Default theme assigned
- Icon: `ThemeIconFA` value from the Themes table (e.g. `fa-snowflake`, `fa-water`)
- Size: `fa-xs` (0.75em)
- Color: `--color-accent` of the poem's theme (not the ambient page theme)
- No label text — icon only
- Tooltip (title attribute): `{ThemeDisplayName}` (e.g. "Winter")
- If the poem has no theme, the badge element is not rendered (not hidden — absent)

### 10.3 Word Links

All words in poem lines are hyperlinks to the word-filtered feed (`/word/{word}`). Stop words (per `StopWords` table) are rendered as plain text, not links.

**Styling:**

| State | Style |
|---|---|
| Default (at rest) | `text-decoration: underline; text-decoration-style: dotted; text-decoration-color: rgba(currentColor, 0.35); text-underline-offset: 3px` |
| Hover | Underline becomes solid; `text-decoration-color: --color-accent`; letter-spacing increases by `0.02em` (subtle expansion); transition `150ms ease` |
| Active/pressed | `color: --color-accent-hover`; underline solid |
| Visited | No special visited styling — poems are not documents; word links should not accumulate visited state visually |

Hashtags (`#word`) receive the same treatment as word links but use `--color-accent` as their default color at rest, not just on hover, to distinguish them as explicit tags.

### 10.4 Composer (`ComposeBox.razor`)

**Collapsed state:** A single rounded-rectangle input row: avatar (32px) + placeholder text "Write a poem…" + `fa-feather-alt` icon right-aligned. Clicking/focusing expands.

**Expanded state (top to bottom):**

1. **Poem input area:** A single `<textarea>`. Lines are delimited by newlines. Font: IM Fell English, 1.25rem. Per-line syllable count badges are displayed inline to the right of each line.

2. **Type badge + Freeform toggle row:** Read-only poem type badge (e.g. "haiku", "tanka") on the left, Freeform toggle on the right. The Freeform toggle is an explicit opt-in that overrides the detected type and bypasses syllable validation.

3. **Count display line:** Character count, total syllable count, word count, and line count in order: `{chars} chars · {syl} syl · {words} words · {lines} lines`. Uses DM Mono, 0.8rem, `--color-text-muted`.

4. **Theme picker row:** Horizontally scrollable chip list of Active themes. Each chip: `fa-{icon}` + theme name + color swatch circle. Selected chip has a solid border in `--color-accent`. Recommendation hint text appears above the row when confidence ≥ threshold.

5. **Action bar:** [Generate `fa-dice`] [Save Draft] [Publish `fa-paper-plane`] — right-aligned. Publish is disabled (not hidden) when syllable validation fails for non-freeform poems, with a tooltip explaining the reason.


**Debounce and paste behavior:**

- Syllable counts, poem type detection, and count display (chars/syl/words/lines): 800ms debounce on keystroke.
- Paste events (`onpaste`): all counts and type detection update after 50ms to allow the DOM to settle.
- Theme recommendation: fires server-side on Save Draft / on a separate 1200ms debounce in the composer (less frequent than syllable updates).

### 10.5 Syllable Count Badge (Composer)

Inline to the right of each line input. DM Mono, 0.8rem. Fixed width to prevent layout shift as numbers change.

Format: `{actual} / {target}` when a target is known; `{actual}` alone for Freeform.

Badge background is transparent; only the text color changes (ok / progress / over). A subtle CSS transition on color (`200ms`) prevents jarring flickers as the user types.

### 10.6 Nav Bar

- Fixed top, height `48px`, background `--color-surface` with `1px` bottom border in `--color-hairline`
- Always renders in Default theme tokens regardless of current poem theme
- Left: `fa-feather-alt` icon (16px, `--color-accent`) + wordmark "Haiku" (Cormorant Garamond, 1.25rem, 600 weight, `--color-text-primary`). Together they form a link to `/`
- Center (desktop): Search bar (DM Sans, 0.9rem)
- Right: Nav items vary by auth state (Login/Register unauthenticated; Feed/Drafts badge/Bookmarks/Profile/Logout authenticated)
- Draft badge: a small pill counter (`fa-circle-dot` or numeric) on the Drafts nav item when the user has unpublished drafts

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

---

## 12. Accessibility Constraints

These extend PRD Section 6.4 with UI-specific detail.

| ID | Requirement | Priority |
|---|---|---|
| ACC-01 | All theme color combinations (text on background, text on card surface) must meet WCAG 2.1 AA contrast ratio: ≥ 4.5:1 for body text, ≥ 3:1 for large text (≥18pt or ≥14pt bold). This is verified for every theme before it is set Active. | MH |
| ACC-02 | The card tint overlay (pseudo-element) must not reduce the effective contrast ratio of poem text below 4.5:1. Tint opacity must not exceed 6% for light mode or 8% for dark mode. | MH |
| ACC-03 | Word links within poems must meet the non-text contrast requirement (≥ 3:1) for the dotted underline against the card background. | MH |
| ACC-04 | Vote buttons (`fa-thumbs-up`, `fa-thumbs-down`) carry `aria-label` attributes: "Upvote poem", "Downvote poem", "Remove upvote", "Remove downvote" (contextual to current state). | MH |
| ACC-05 | The heart/love toggle carries `aria-label`: "Love this poem" / "Remove love". The bookmark toggle: "Bookmark poem" / "Remove bookmark". | MH |
| ACC-06 | The syllable count badge in the composer carries `aria-live="polite"` so screen readers announce count changes without interrupting typing. | MH |
| ACC-07 | The scroll-driven theme transition does not produce flashing or rapid color alternation. Minimum transition duration is 400ms between any two theme states. | MH |
| ACC-08 | Focus rings are visible in all themes and all modes. The ring color must have ≥ 3:1 contrast against both the card background and the page background. Use `--color-accent` with `outline: 2px solid; outline-offset: 2px`. | MH |
| ACC-09 | The impersonation banner (amber, DEV-10 in PRD) must meet contrast requirements in both light and dark mode. It is exempt from theme transitions — it always renders in a fixed amber/black combination. | MH |

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

---

## 15. Open UI Questions

| # | Question | Owner | Status |
|---|---|---|---|
| UI-OQ-01 | Should light/dark mode preference be stored in the user's account (server-side) or only in localStorage / system preference? | Product / Eng | Open |
| UI-OQ-02 | Should unauthenticated users be able to set a preferred color mode, or is system preference (`prefers-color-scheme`) the only signal for guests? | Product | Open |
| UI-OQ-03 | What is the visual treatment for a poem marked NSFW by a moderator but not hidden — blur, content warning overlay, or collapse? | Product | Open |
| UI-OQ-04 | Should the word cloud (sidebar) visually reflect the current theme's accent color, or always render in Default tokens? | Design | Open |
| UI-OQ-05 | On the onboarding flow, should the platform demonstrate the theme system to new users as part of the welcome experience? | Product | Open |
| UI-OQ-06 | Should the theme recommendation hint in the composer be suppressible globally by a user preference ("never suggest themes")? | Product | Open |
| UI-OQ-07 | How should the admin theme editor handle the AnimationKey field — free text, a dropdown of registered animation keys, or both? | Engineering | Open |
| UI-OQ-08 | What is the scroll driver behavior when the user navigates to a word-filtered or tag-filtered feed? All cards may share a single theme. | Engineering | Open |
