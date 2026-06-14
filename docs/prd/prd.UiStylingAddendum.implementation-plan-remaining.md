# Remaining Implementation Plan — UI Styling Addendum

> Companion to `prd/prd.UiStylingAddendum.md` (annotated June 2026)
> Generated: June 2026

---

## Table of Contents

A. [Ordered Implementation Plan](#a-ordered-implementation-plan)
B. [Ambiguous Items — Approaches](#b-ambiguous-items--approaches)
C. [General UI Consistency Improvements](#c-general-ui-consistency-improvements)
D. [Improvements for Addressed Items](#d-improvements-for-addressed-items)

---

## A. Ordered Implementation Plan

### Phase 0: Foundation (must ship before any other work)

| Order | Item | ID | Effort | Depends On |
|-------|------|----|--------|------------|
| 0.1 | Add `Guid? ThemeId` FK to `Poem.cs` | TH-04 | 2h | — |
| 0.2 | Configure FK in `HaikuDbContext.OnModelCreating` | TH-04 | 30m | 0.1 |
| 0.3 | Generate EF Core migration for Themes + ThemeKeywords + Poem.ThemeId | TH-01/02/04 | 1h | 0.2 |
| 0.4 | Create seed data: Default (non-deletable), Winter, Spring, Summer, Autumn with keywords | TH-03 | 2h | 0.3 |
| 0.5 | Update `CreatePoemCommand` / `CreatePoemCommandHandler` to accept and store `ThemeId` | TH-18 | 1h | 0.1 |
| 0.6 | Add `ThemeId` to `PoemDefinition` value object | TH-18 | 15m | 0.1 |

**Rationale**: Without Phase 0, no poem can be associated with a theme at the data level,
theme rendering cannot be tested end-to-end, and the ComposeBox publish flow has no backend.

### Phase 1: Theme CSS Generation (unlocks admin extensibility)

| Order | Item | ID | Effort | Depends On |
|-------|------|----|--------|------------|
| 1.1 | Create `ThemeStyleService` that reads `LightPaletteJson`/`DarkPaletteJson` from DB and emits CSS `[data-theme]` rule blocks as a string | TH-25 | 4h | 0.3 |
| 1.2 | Create a Blazor component `<ThemeStyleSheet>` that calls `ThemeStyleService` and renders a `<style>` tag in the `<head>` via `HeadOutlet` | TH-25 | 2h | 1.1 |
| 1.3 | Hydrate `ThemeStyleSheet` lazily — only emit CSS for themes present in the current viewport's poems | TH-25 | 3h | 1.2 |
| 1.4 | Remove hardcoded `[data-theme]` blocks from `app.css` (keep Default `:root` and `[data-color-scheme]`) | TH-25 | 30m | 1.2 |
| 1.5 | Add fallback: on first load, emit all Active theme CSS to prevent flash; then refine lazily | TH-25 | 1h | 1.3 |

**Rationale**: Without Phase 1, admin-created themes will never render on the frontend
because their palette data only exists in the DB, not in CSS.

### Phase 2: Navbar & Layout Isolation

| Order | Item | ID | Effort | Depends On |
|-------|------|----|--------|------------|
| 2.1 | Move `<NavMenu />` outside `#haiku-app` wrapper — it becomes a sibling, not a child | SC-DR-05 | 30m | — |
| 2.2 | Set explicit `data-theme="default"` on a new `<div id="main-content">` that wraps just `@Body` | SC-DR-05 | 15m | 2.1 |
| 2.3 | Update scroll driver to observe `#main-content` instead of `#haiku-app` | SC-DR-05 | 15m | 2.2 |
| 2.4 | Set navbar to `position: fixed; top: 0; height: 48px` with `z-index: 1030` | §10.6 | 1h | — |
| 2.5 | Add `<body>` padding-top of 48px to account for fixed navbar | §10.6 | 15m | 2.4 |
| 2.6 | Add `min-width` / fixed width to `.syllable-badge` to prevent layout shift | §10.5 | 15m | — |

**Rationale**: Fixing the navbar layout and theme isolation resolves a spec compliance issue
and a visual bug (navbar height, fixed positioning, theme bleed-through).

### Phase 3: Composer Backend Wiring

| Order | Item | ID | Effort | Depends On |
|-------|------|----|--------|------------|
| 3.1 | Wire ComposeBox Publish button to `CreatePoemCommand` via injected `IMediator` | TH-18 | 3h | 0.5 |
| 3.2 | Wire Save Draft button (same command with `IsDraft: true`) | TH-18 | 1h | 0.5 |
| 3.3 | Read real Active themes from `ThemeService.GetActiveThemesAsync()` in ComposeBox | TH-13 | 1h | 0.4 |
| 3.4 | Add collapsed/expanded two-state toggle to ComposeBox (single-line prompt) | §9.2 | 2h | — |
| 3.5 | Add ComposeBox debounce: 800ms keystroke, 50ms paste, 1200ms recommendation | TH-11 | 3h | — |
| 3.6 | Wire `ThemeRecommendationService` recommendation on every debounced input (not just first render) | TH-11 | 1h | 3.5 |
| 3.7 | Add `aria-label` attributes to vote/heart/bookmark buttons (replace `title`) | ACC-04/05 | 30m | — |

### Phase 4: Feed Data Integration

| Order | Item | ID | Effort | Depends On |
|-------|------|----|--------|------------|
| 4.1 | Create `GetFeedQuery` + `GetFeedQueryHandler` slice returning poems with vote/love/bookmark counts | — | 4h | 0.5 |
| 4.2 | Update `Feed.razor` to use `IMediator.Send(new GetFeedQuery())` instead of hardcoded data | — | 2h | 4.1 |
| 4.3 | Wire HaikuCard `OnVote` to `CastVoteCommand` | — | 1h | — |
| 4.4 | Wire HaikuCard `OnLoveToggle` to `AddLoveCommand` / `RemoveLoveCommand` | — | 1h | — |
| 4.5 | Wire HaikuCard `OnBookmarkToggle` to `AddBookmarkCommand` / `RemoveBookmarkCommand` | — | 1h | — |
| 4.6 | Add draft badge counter to navbar for authenticated users (count of `IsDraft && AuthorId == currentUser`) | §10.6 | 1h | — |
| 4.7 | Add card tint `background-color` live preview on theme chip selection in ComposeBox | TH-17 | 2h | — |

### Phase 5: Interaction Animations

| Order | Item | ID | Effort | Depends On |
|-------|------|----|--------|------------|
| 5.1 | Add `animate-micro-bounce` CSS class to vote/bookmark buttons on click (toggle class, remove after 200ms) | §10.1 | 1h | — |
| 5.2 | Add `animate-heart-spin` CSS class to love button on click | §10.1 | 1h | — |
| 5.3 | Add `animate-score-flash-green/red` CSS class to net score span on vote | §10.1 | 1h | — |
| 5.4 | Add button flash feedback on CopyFormat methods (brief background color pulse) | §10.1 | 1h | — |

### Phase 6: Ambient Animation Integration

| Order | Item | ID | Effort | Depends On |
|-------|------|----|--------|------------|
| 6.1 | Add `<div id="ambient-animation-layer" style="position: fixed; inset: 0; pointer-events: none; overflow: hidden; z-index: 0;">` to `MainLayout.razor` outside themed content | AN-02 | 15m | — |
| 6.2 | Create `AmbientAnimationManager` Blazor component that observes `data-theme` changes on `#haiku-app` and calls `haikuAnimations.start()`/`stop()` | AN-07 | 3h | 6.1 |
| 6.3 | Cap animation layer position to main content column (not navbar/sidebar) via CSS clip or JS bounding rect | AN-02 | 2h | 6.1 |
| 6.4 | Implement fade-out/fade-in for outgoing/incoming animations during theme transitions | AN-05 | 3h | 6.2 |
| 6.5 | Add `window.innerWidth >= 768` check to animation engine start function | §13 | 15m | — |
| 6.6 | Implement `AnimationIntensity` — "Moderate" doubles particle count, adds 0.02 to max opacity | §7 | 1h | 6.2 |

### Phase 7: Scroll Driver Interpolation

| Order | Item | ID | Effort | Depends On |
|-------|------|----|--------|------------|
| 7.1 | Create `getThemePalette(themeKey)` JS function that reads `LightPaletteJson` from inline `<script>` data or fetches from server | SC-DR-02/03 | 4h | 1.2 |
| 7.2 | Implement `lerpColor(hex1, hex2, t)` function that interpolates hex colors channel-wise | SC-DR-02/03 | 1h | — |
| 7.3 | Update `blendThemes()` to compute weighted-average palette of two visible themes and apply as inline `style` on root element | SC-DR-02/03 | 4h | 7.1, 7.2 |
| 7.4 | Add CSS `transition: none` on root element during JS-driven interpolation to prevent fighting CSS transitions | SC-DR-02/03 | 15m | 7.3 |

**Note on SC-DR-02/03 complexity**: This is the single most technically challenging
unimplemented feature. The interpolation must:
1. Know the complete palette (all ~15 tokens) for each theme
2. Compute weighted medians between two palettes every animation frame
3. Apply 15+ CSS custom properties on every blend
4. Handle the edge case where a theme only defines partial tokens (inherit from Default)

Consider deferring this to a dedicated performance-optimization pass. The current
CSS transition-based snap provides acceptable UX at much lower complexity.

### Phase 8: Admin Theme Management

| Order | Item | ID | Effort | Depends On |
|-------|------|----|--------|------------|
| 8.1 | Add `manage_themes` privilege to `PrivilegeNames.cs` | TH-19 | 15m | — |
| 8.2 | Add "Theme Management" section to `Admin.razor` with list of all themes | TH-19 | 2h | — |
| 8.3 | Create theme create/edit form: DisplayName, IconFA, Status dropdown, HasAnimation, AnimationKey, AnimationIntensity, per-token color pickers (light + dark) | TH-20 | 6h | — |
| 8.4 | Add ThemeKeyword management per theme: add/remove/edit weight | TH-21 | 3h | — |
| 8.5 | Implement status transition validation: Draft→Active→Archived→Active (block Draft→Archived) | TH-22 | 1h | — |
| 8.6 | Create theme preview panel rendering a static example poem card in light/dark | TH-23 | 3h | — |
| 8.7 | Block deletion of themes with poem associations (check count, show Archive prompt) | TH-19 | 1h | 0.1 |

### Phase 9: Pages & Routing

| Order | Item | ID | Effort | Depends On |
|-------|------|----|--------|------------|
| 9.1 | Add `/poem/{id}` route in `App.razor`, create `PoemDetail.razor` page | SC-DR-06, §9.3 | 4h | 0.1 |
| 9.2 | Apply full theme (color + animation) immediately on `/poem/{id}` load | SC-DR-06 | 1h | 9.1, 6.2 |
| 9.3 | Update `PoetPage.razor` with spec-compliant two-column layout (col-md-4 metadata + col-md-8 feed) | §9.4 | 3h | — |
| 9.4 | Add 429 status code handling in `Status.razor` (ensure `UseStatusCodePagesWithReExecute` covers it) | §10.7 | 30m | — |

### Phase 10: Accessibility & Polish

| Order | Item | ID | Effort | Depends On |
|-------|------|----|--------|------------|
| 10.1 | Implement impersonation banner (amber, fixed-top, WCAG-compliant) | ACC-09 | 2h | — |
| 10.2 | Create `StopWords` table, entity, and repository. Replace hardcoded `HashSet` in HaikuCard | §10.3 | 2h | — |
| 10.3 | Split `app.css` into spec-proposed file structure (themes.css, typography.css, navbar.css, word-link.css, animations/*.css) | §14.1 | 2h | — |
| 10.4 | Add automated WCAG AA contrast verification script for themes (e.g., using `axe-core` CLI or `contrast-checker` npm) | ACC-01 | 3h | — |
| 10.5 | Add `data-color-scheme` attribute to `#haiku-app` for self-contained component theming | §14.2 | 15m | — |
| 10.6 | Add `--color-surface-raised` usage to sidebar and compose box | §4 | 15m | — |
| 10.7 | Ensure `429` is routed by checking `UseStatusCodePagesWithReExecute` pattern | §10.7 | 15m | — |

### Phase 11: Future / Post-MVP

| Order | Item | ID | Effort | Depends On |
|-------|------|----|--------|------------|
| 11.1 | Community theme proposals with moderation queue | TH-24 | 8h | 8.x |
| 11.2 | Theme recommendation suppression user preference | UI-OQ-06 | 2h | — |
| 11.3 | Theme onboarding demo for new users | UI-OQ-05 | 4h | — |

---

## B. Ambiguous Items — Approaches

### B.1 TH-17: Live Theme Preview in Composer

**Problem**: When the author selects a different theme chip in the composer, the composer
card should preview that theme's palette. But the ambient animation is not previewed.

**Approach 1 — data-theme swap on composer wrapper (Recommended)**
Add a `data-theme` attribute to the composer card wrapper. On chip select, update the
attribute to the selected theme's key. The CSS cascade immediately applies the new palette.
Animation overlay is not mounted inside the composer (gated by component check).
- Pros: Zero JS, immediate, leverages existing CSS cascade
- Cons: Theme must be in CSS (requires Phase 1), only works for hardcoded themes initially

**Approach 2 — inline style injection**
Compute the palette values server-side and set them as inline `style="--color-accent: ..."` on the composer element. No CSS dependency.
- Pros: Works without Phase 1 (CSS generation)
- Cons: Duplicates CSS cascade logic in C#, more complex, inline styles harder to debug

**Approach 3 — full-page preview**
Navigate to a preview route like `/preview?poem=...&theme=winter` that renders the poem
in full theme context.
- Pros: Author sees exactly what readers will see
- Cons: Disruptive UX (page navigation), slow, over-engineered for an inline preview

**Recommendation**: Approach 1. It's the simplest and most maintainable. Pair with Phase 1
so non-hardcoded themes also preview correctly.

---

### B.2 SC-DR-02/03: Full Palette Interpolation

**Problem**: The spec requires continuous crossfade between two themes weighted by
intersection ratio, with per-channel R/G/B hex interpolation.

**Approach 1 — JS Interpolation with Inline Styles (Spec-aligning)**
Maintain a JS-side map of all theme palettes. When two poems overlap, compute weighted
median for every `--color-*` token and apply as inline styles on `#haiku-app`.
- Pros: Matches spec exactly, smooth visual result
- Cons: High complexity (15+ tokens × 2 channels × 60fps = 1800+ DOM writes/sec),
  theme palette data must be available to JS (inline or fetched),
  fighting CSS transitions requires disabling them during interpolation

**Approach 2 — CSS Transition Snap (Current)**
The scroll driver identifies the dominant theme and sets `data-theme`. CSS `transition`
on `background-color: 0.4s ease` smooths the change. No per-channel interpolation.
- Pros: Works now, zero additional complexity, no DOM thrashing
- Cons: Does not produce intermediate blended colors; one theme cuts to the next

**Approach 3 — Hybrid Snap with Overshoot**
Same as Approach 2 but add a `transition-delay` on the outgoing theme so the change
takes 600ms instead of 400ms, creating a longer crossfade. No interpolation.
- Pros: Minimal complexity, visually softens the snap
- Cons: Still not true interpolation; 600ms delay may feel sluggish

**Recommendation**: Implement Approach 3 immediately (trivial CSS change). Defer Approach 1
for a dedicated performance pass. The current CSS transition (400ms) already provides
acceptable UX — the marginal improvement of per-channel interpolation is not worth the
complexity at this stage.

**Developer expectation**: The 400ms CSS transition is perceptible to attentive users
but not jarring. Full interpolation would be noticeable mainly on slow scrolls where
two poems share the viewport equally for >1 second. This is an edge case.

---

### B.3 TH-06: Archived Theme Enforcement

**Problem**: Archived themes should not accept new poem associations. This requires
business logic enforcement at multiple layers.

**Approach 1 — Service-only gate (Recommended)**
In `PoemService.CreateAsync` / `CreatePoemCommandHandler`, check if the assigned theme's
`Status == "Archived"` and throw/reject.
- Pros: Single enforcement point, testable, simple
- Cons: Does not prevent direct DbContext usage (test projects, future code)

**Approach 2 — Repository gate**
Add a check in `IPoemRepository.SaveAsync` that validates the theme status before
persisting.
- Pros: Catches all write paths
- Cons: Repository should not contain business logic; violates separation of concerns

**Approach 3 — Database CHECK constraint**
Add a `CHECK` constraint: `Status != 'Archived' OR PoemId IS NULL`. But this requires
a cross-table constraint or a trigger, which EF Core doesn't support natively.
- Pros: Enforces at DB level, cannot be bypassed
- Cons: Not portable, complex to manage, EF Core schema validation may not reflect it

**Recommendation**: Approach 1. Check `theme.Status` before creating the poem. Add a
unit test that verifies the rejection. This is sufficient for v1.0.

**User expectation**: If an admin sets a theme to Archived while an author is composing
with that theme selected, the publish will fail with a clear error message.

---

### B.4 TH-10: Configurable Recommendation Threshold

**Problem**: The threshold should come from `appsettings.json` rather than being hardcoded.

**Approach 1 — IOptions pattern (Recommended)**
Create `ThemeRecommendationOptions` class bound to `Themes:RecommendationThreshold`
configuration section. Inject `IOptions<ThemeRecommendationOptions>` into the service.
- Pros: .NET standard pattern, hot-reloadable in development, testable via options mocking
- Cons: Requires more DI boilerplate

**Approach 2 — IConfiguration direct injection**
Inject `IConfiguration` and read the value directly in the service method.
- Pros: Less boilerplate
- Cons: Harder to unit test, stringly-typed key, not hot-reloadable

**Approach 3 — Static config class**
Create a `ThemeConfig` static class with a settable property. Configure in `Program.cs`.
- Pros: Simplest possible, no DI needed
- Cons: Static mutable state, not testable in parallel, not hot-reloadable

**Recommendation**: Approach 1. It's the standard .NET pattern and aligns with how the
rest of the app handles configuration (e.g., email provider selection in Program.cs).

---

### B.5 AN-05: Animation Crossfade During Theme Transitions

**Problem**: When scrolling between poems with different animations (e.g., Winter/snowfall
to Summer/fireflies), the outgoing animation should fade out as the incoming fades in.

**Approach 1 — Opacity tween on canvas (Recommended)**
Track a `crossfadeProgress` (0 to 1) in the scroll driver. Pass it to the animation engine.
The engine sets CSS `opacity` on each canvas: outgoing = `0.18 × (1 - progress)`,
incoming = `0.18 × progress`. After progress reaches 1, destroy the outgoing canvas.
- Pros: Smooth crossfade between any two animations
- Cons: Requires both canvases to exist simultaneously (memory), more complex lifecycle

**Approach 2 — One canvas, instant switch**
The outgoing animation clears its canvas immediately. The incoming animation starts fresh.
The CSS transition on the overlay container's opacity provides the crossfade.
- Pros: Simple, no dual-canvas management
- Cons: Brief moment with no animation visible during the transition

**Approach 3 — Deferred stop/start with overlap window**
When a new theme becomes dominant, wait 400ms before stopping the old animation, then
start the new one. During the 400ms overlap, the old animation gradually stops producing
new particles (set spawn rate to 0, let existing particles decay naturally).
- Pros: Smoother than instant switch, no dual-canvas requirement
- Cons: Particle decay takes time; gaps may appear; harder to reason about

**Recommendation**: Approach 1. The opacity tween is clean, matches the color transition
behavior, and gives the best visual result. The 200px-margin bounding area keeps canvas
sizes manageable, so dual-canvas memory impact is negligible.

---

### B.6 TH-24: Community Theme Proposals (Future)

**Problem**: Spec says this is for a future version. No commitment to any version.

**Approach — Don't implement yet.**
If/when scoped, the architecture should:
1. Create a `ThemeProposal` table (fields: Theme JSON, proposed by UserId, status)
2. Add a `manage_themes` privilege for review
3. Reuse the admin preview panel (TH-23) for moderation review
4. On approval, create the Theme record from the proposal JSON

No implementation recommended for v1.0.

---

### B.7 UI-OQ-04: Word Cloud Theme Colors

**Problem**: Should the word cloud reflect the current theme's accent color or always
use Default tokens?

**Approach 1 — Use theme accent (current implementation)**
Word cloud badges use `color-mix(in srgb, var(--color-accent) 10%, var(--color-surface))`.
They naturally pick up the scroll-driven theme accent.
- Pros: Visually cohesive, the widget feels like part of the theme
- Cons: Word cloud in Dark theme with a Summer accent may look mismatched

**Approach 2 — Always Default tokens**
Use fixed `--color-accent` (Default theme's blue) for word cloud badges regardless of
current theme. Explicitly wrap sidebar in `data-theme="default"`.
- Pros: Stable visual, word cloud is always readable in the site's brand color
- Cons: Disconnected from the ambient theme, feels like a separate product

**Approach 3 — Widget-level theme override with user setting**
Let users decide in Settings whether widget reflects theme or stays Default.
- Pros: Maximum flexibility
- Cons: Over-engineered, adds UI surface for trivial preference

**Recommendation**: Approach 1 (keep current). The spec's answer at UI-OQ-04 is "Open"
but the current behavior (theme-aware) feels more cohesive. If Product decides otherwise,
the fix is a single CSS override on `.sidebar-widget`.

---

### B.8 UI-OQ-07: AnimationKey Editor UX

**Problem**: When the admin theme editor lets users pick an animation, should the
AnimationKey field be free text, a dropdown, or both?

**Approach 1 — Dropdown of registered animation keys (Recommended)**
The admin editor queries `Object.keys(window.haikuAnimations)` (filtered to exclude
internal methods like `start`, `stop`, `createCanvas`). Renders as a `<select>`.
- Pros: Prevents typos, discoverable, shows which animations are available
- Cons: Requires JS interop from admin page, animation keys must be in a known list

**Approach 2 — Free text with validation**
Text field accepting any string. On save, validate that the key exists in the animation
registry. Show error if not found.
- Pros: Flexible, no JS interop needed for rendering
- Cons: Error-prone, poor discoverability

**Approach 3 — Both: text field + "Test" button**
Free-text field plus a "Preview" button that triggers the animation briefly so the admin
can see what it looks like.
- Pros: Best UX for admin who may not know animation names
- Cons: Most complex to implement, requires animation preview in admin panel

**Recommendation**: Approach 1 (dropdown). Hardcode the known keys in a `List<string>` on
the admin edit page C# code. Update the list when new animations are added. This avoids
JS interop entirely and is simple.

---

## C. General UI Consistency Improvements

### C.1 Font Family Consistency

**Issue**: Sections of the UI still use Bootstrap's default system font (`-apple-system,
BlinkMacSystemFont, "Segoe UI", etc.`) instead of DM Sans.

**Fixes**:
| Location | Current Font | Should Be | Fix |
|----------|-------------|-----------|-----|
| `badge` elements | System font | DM Sans (via `--bs-font-sans-serif`) | CSS var already remapped — verify no overrides |
| `blockquote` elements | System font or browser default | DM Sans | Add `blockquote { font-family: var(--font-ui) }` |
| `Status.razor` `.status-code` | System font | DM Mono | Currently uses `.status-code { font-family: var(--font-mono) }` — DONE |
| `input`, `textarea` (non-composer) | System font | DM Sans | Add global `input, textarea, select { font-family: var(--font-ui) }` |
| `HeroSection` `h1` | Cormorant Garamond | Cormorant Garamond | DONE — `.hero-section h1 { font-family: var(--font-display) }` |

### C.2 Card Consistency

**Issue**: Home.razor cards use `haiku-card` class (the new standard) but some pages/potential
future cards might use Bootstrap's `.card`. All cards should look consistent.

**Fixes**:
- Override Bootstrap `.card` to match `haiku-card` visual treatment:
  ```css
  .card {
    border-top: 4px solid var(--color-hairline);
    border-radius: 0;
    box-shadow: 0 1px 3px rgba(0,0,0,0.06);
  }
  ```
  This is already done in `app.css`.

### C.3 Button Consistency

**Issue**: Buttons use a mix of:
- `<button class="btn btn-primary">` (Bootstrap primary — accent color)
- `<button class="btn btn-sm btn-link nav-link border-0">` (nav-style button)
- `<button class="action-btn">` (custom HaikuCard button)

These three button styles look different. Action buttons in HaikuCard don't match
Bootstrap's button styling.

**Fix**: Standardize on two button patterns:
1. **Primary actions**: `.btn-primary` (Bootstrap, accent-filled)
2. **Card actions**: `.action-btn` (text-only with icon, no background)
3. **Nav actions**: `.btn-link` styled as nav-link

This is acceptable for three distinct contexts. No change needed.

### C.4 Color Token Coverage

**Issue**: Some Bootstrap components use CSS variables not remapped to our token system.

| Bootstrap Variable | Remapped? | Fix if missing |
|-------------------|-----------|----------------|
| `--bs-body-bg` | YES | — |
| `--bs-body-color` | YES | — |
| `--bs-primary` | YES | — |
| `--bs-border-color` | YES | — |
| `--bs-link-color` | YES | — |
| `--bs-link-hover-color` | YES | — |
| `--bs-secondary-color` | NO | Add `--bs-secondary-color: var(--color-text-muted)` |
| `--bs-success` | NO | Add `--bs-success: var(--color-syllable-ok)` |
| `--bs-warning` | NO | Add `--bs-warning: var(--color-syllable-progress)` |
| `--bs-danger` | NO | Add `--bs-danger: var(--color-score-negative)` |
| `--bs-info` | NO | Add `--bs-info: var(--color-accent)` |
| `--bs-light` | NO | Should point at `--color-surface` |
| `--bs-dark` | NO | Should point at `--color-text-primary` |

**Fix**: Add the missing 7 remaps in `app.css` `:root` block.

### C.5 Focus Ring Audit

**Issue**: The spec requires `outline: 2px solid var(--color-accent); outline-offset: 2px`
for focus rings (ACC-08). Current implementation uses this for `:focus` on `btn` and
`form-control` but may miss some elements.

**Elements to audit**:
- [ ] `.nav-link` (navbar links)
- [ ] `.action-btn` (card action buttons)
- [ ] `a` elements (word-links, hashtags)
- [ ] `.theme-chip` buttons
- [ ] `.dropdown-item`
- [ ] `.trending-tag` / `.word-cloud-badge`

**Fix**: Add a global focus rule:
```css
:focus-visible {
  outline: 2px solid var(--color-accent);
  outline-offset: 2px;
}
```

### C.6 Spacing Audit

**Issue**: Inconsistent vertical rhythm.

| Component | Current Margin | Spec Target | Fix |
|-----------|---------------|-------------|-----|
| Between poem cards | `1.5rem` | `1.5rem` | DONE |
| Card body padding (desktop) | `1.5rem` | `1.5rem` | DONE |
| Card body padding (mobile) | `1rem` | `1rem` | DONE |
| Navbar bottom border | `1px` | `1px` | DONE |
| Sidebar top sticky offset | `64px` | `64px` | DONE (but depends on fixed navbar — see Phase 2) |
| Section title bottom margin | `1.5rem` | `1.5rem` | DONE |
| Hero section padding | `4rem 0` | unspecified | Acceptable |

### C.7 Dark Mode Data Attribute Duplication

**Issue**: The app uses both `data-bs-theme` (set by JS on `<html>`) and
`data-color-scheme` (set by our JS on `<html>`). These serve the same purpose.

**Fix**: Standardize on `data-color-scheme` for our token system. Remove
`data-bs-theme` from `haiku-theme.js` since Bootstrap 5.3's dark mode is
overridden by our CSS variable remapping anyway. Keep `data-bs-theme` only if
Bootstrap's built-in dark mode components are desired (they're not — we remap
all variables).

---

## D. Improvements for Addressed Items

### D.1 app.css (already implemented)

**Current state**: A single large file (300+ lines) combining imports, tokens,
typography, theme overrides, animations, component styles, and responsive rules.

**Improvements**:
1. **Split into spec file structure** (§14.1). Move theme overrides to
   `themes.css`, typography to `typography.css`, animation keyframes to
   `animations/`, navbar rules to `components/navbar.css`, word-link rules to
   `components/word-link.css`. Load each via separate `<link>` in `App.razor`.
   *Why*: Maintainability — a developer can find and edit the relevant file
   without scanning 300 lines.
2. **Remove duplicated Bootstrap CDN `@import`**. The `@import` in `app.css`
   duplicates the `<link>` in `App.razor`. Keep the App.razor `<link>` and
   remove the `@import` to avoid double-loading Bootstrap (minor perf gain).
3. **Add source-map friendly organization**. Add section comments like
   `/* ===== Base Tokens ===== */` to aid navigation even before the split.
4. **Audit unused CSS**. Check for dead code like `.brutalist-card`,
   `.neo-brutalist-card`, `.brutalist-title`, `.hero-section gradient`,
   etc. These were in the original `app.css` before the rewrite and may still
   exist as dead code. Remove or migrate to the new system.
5. **Add `--color-surface-raised` usage**. The token exists but is not applied
   to sidebar or compose box backgrounds. Use it:
   ```css
   .sidebar-widget,
   .compose-box {
     background-color: var(--color-surface-raised);
   }
   ```

### D.2 Theme Entities (Theme.cs, ThemeKeyword.cs)

**Current state**: All spec columns present. JSON columns store palette data
but are unused in rendering. No validation attributes beyond `[Required]` and
`[MaxLength]`.

**Improvements**:
1. **Add domain validation** for `Status` — restrict to "Draft", "Active",
   "Archived" via a `ThemeStatus` enum or `[RegularExpression]`.
2. **Add `AnimationIntensity` enum** — create `AnimationIntensity` enum
   (Subtle, Moderate) instead of free string.
3. **Add JSON serialization for LightPalette/DarkPalette**. Create a
   `ThemePalette` record class and use a value converter to serialize/deserialize
   to/from JSON automatically. This makes working with palette data type-safe.
4. **Add non-deletable marker**. Add an `IsBuiltIn` bool column. On delete,
   check `IsBuiltIn` and reject. Set `IsBuiltIn = true` for Default theme.

### D.3 ThemeRecommendationService

**Current state**: Keyword matching with word-count normalization. Works
correctly for exact keyword matches. Threshold is hardcoded at 0.55.

**Improvements**:
1. **Add stemming/lemmatization**. Current matching requires exact keyword
   matches ("snow" won't match "snowy" or "snowing"). Use a simple stemming
   library or implement a basic Porter stemmer to improve recall.
2. **Add `IConfiguration` / `IOptions` binding** for threshold (see B.4).
3. **Add word-weight boosting**. Not all words are equally thematic — content
   words (nouns, adjectives) should be weighted higher than function words.
   Currently all words are equal. Add a `BoosterWords` list of known thematic
   stems and boost their match weight by 1.5x.
4. **Add multi-word keyword support**. Current implementation matches single
   words only. "northern lights" as a keyword would never match. Add phrase
   detection.
5. **Add `ILogger` structured logging** for debugging recommendations in
   production. Log which keywords matched for a given poem text.

### D.4 HaikuCard.razor

**Current state**: All structural elements present. Interactions use
EventCallback patterns. Stop words are hardcoded. Theme badge is conditional.

**Improvements**:
1. **Replace `title` with `aria-label`** on all interactive buttons (ACC-04/05).
2. **Add `@key` to `@foreach` loops** to prevent unnecessary re-renders on
   poem lines and word lists. Currently missing `@key` on both loops.
3. **Add `<img>` `alt` attribute** for avatar image. Currently `alt=""` which
   is acceptable for decorative images, but should include author name:
   `alt="@AuthorDisplayName's avatar"`.
4. **Consolidate overflow menu actions** into a reusable service or component.
   Share/Report/CopyText logic is duplicated inline. Extract to
   `PoemActionsService` or `OverflowMenu.razor`.
5. **Add tooltip component** instead of `title` attribute. The spec says
   "tooltip: Log in to vote" — use Bootstrap's tooltip JS instead of
   native `title` for consistent styling.
6. **Add `prefers-reduced-motion` JS check** before applying animation CSS
   classes. Currently the CSS handles this via media query, which is correct
   — but the JS should also skip adding classes for belt-and-suspenders.

### D.5 ComposeBox.razor

**Current state**: Core structure present. Syllable analysis via PoemEngine.
Theme recommendation hint appears on first render. No debounce.

**Improvements**:
1. **Add proper debounce** (see A.3.5). Use `CancellationTokenSource` pattern
   with `async/await` delay instead of `System.Threading.Timer` for cleaner
   async lifecycle.
2. **Extract `GetSyllablePattern`** into a shared service. It's duplicated
   between `ComposeBox.razor` and `PoemEngine.cs` (private method). Create
   a `SyllablePatternProvider` singleton.
3. **Add `onpaste` handler** with 50ms delay as spec'd. Currently uses
   `@bind:event="oninput"` which fires on paste but without delay.
4. **Add character countdown**. Currently shows `CharCount`. Should add
   a visual indicator when approaching the 500-char limit (e.g., turn orange
   at 450, red at 490).
5. **Add keyboard shortcut** for Publish (Ctrl+Enter) and Save Draft (Ctrl+S).

### D.6 Theme Picker

**Current state**: Chip row with icons, swatches, selection state. Recommendation
hint text. Not yet wired to real data.

**Improvements**:
1. **Extract standalone `ThemePicker.razor`** usage from ComposeBox (currently
   the picker markup is inline in ComposeBox even though `ThemePicker.razor`
   exists as a separate file). Use the component.
2. **Add pulse-ring animation** to recommended chip (TH-15).
3. **Add "No theme" option** as the first chip that sends `null` (TH-16).
4. **Keyboard navigation** — chips should be navigable via arrow keys.

### D.7 Feed.razor

**Current state**: Uses hardcoded `PoemDisplayModel` data. Scroll driver
initialization and disposal. Sidebar with hardcoded WordCloud + Trending.

**Improvements**:
1. **Replace hardcoded data** with real queries from repositories (Phase 4).
2. **Add infinite scroll** or "Load More" pagination. Currently shows 20 poems
   max (or 3 in hardcoded mode). Add `_nextCursor` tracking.
3. **Add optimistic UI updates** for vote/love/bookmark. The current
   EventCallback pattern fires and waits for parent to refresh data. Should
   update the local count immediately and revert on failure.
4. **Add loading skeleton** for initial feed load — show 3 skeleton card
   placeholders while data loads.

### D.8 Scroll Driver (haiku-scroll-driver.js)

**Current state**: IntersectionObserver with multi-threshold. Snaps to dominant
theme. Handles reduced-motion. Clean destruction.

**Improvements**:
1. **Add `data-theme` to `data-theme-card` mapping**. Currently the scroll
   driver reads `data-theme-card` from poem cards but sets `data-theme` on
   `#haiku-app`. If a card has `data-theme-card="winter"`, it maps directly.
   But the card's ThemeKey could be a GUID. Need a JS map: `{ guid -> "winter" }`.
2. **Add intersection change debounce**. The multi-threshold observer fires
   many callbacks during a scroll. The `scheduleBlend` approach is correct,
   but could be further optimized by skipping blends when the dominant theme
   hasn't changed.
3. **Add viewport-based animation suppression**. If the user hasn't scrolled
   for 5 seconds, stop firing blend calculations (themes won't change anyway).

### D.9 Ambient Animations (haiku-animations.js)

**Current state**: 5 animation types implemented. Canvas-based, 30fps throttled,
pointer-events: none, max opacity 0.18.

**Improvements**:
1. **Add mobile suppression** — check `window.innerWidth` before starting (`< 768` → skip).
2. **Add `AnimationIntensity` support** (see B.3.6).
3. **Add device pixel ratio handling**. Current canvas size is in CSS pixels.
   On retina displays, the canvas should be scaled by `window.devicePixelRatio`
   for crisp rendering.
4. **Add particle count limits** to prevent memory leaks. Cap at 100 particles
   total. Evict oldest particles if limit reached.
5. **Add `requestAnimationFrame` pause when tab is hidden**. Use
   `document.hidden` / `visibilitychange` to pause animation when the tab is
   backgrounded.

### D.10 HaikuTheme JS (haiku-theme.js)

**Current state**: getTheme/setTheme with localStorage. Sets both
`data-bs-theme` and `data-color-scheme`.

**Improvements**:
1. **Remove `data-bs-theme` set** (see C.7).
2. **Add system preference change listener**. When the user changes OS-level
   color scheme (`matchMedia("(prefers-color-scheme: dark)").addEventListener`),
   update the theme if the user hasn't overridden it.
3. **Add theme transition class**. Add a `.theme-transitioning` class to
   `<html>` that persists for 400ms after a theme change, enabling CSS
   transitions. Remove after 400ms to prevent future transitions from
   being animated (performance).
