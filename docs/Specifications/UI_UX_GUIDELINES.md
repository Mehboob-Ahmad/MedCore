# 🎨 UI/UX Guidelines — MedicHp Digital Healthcare Ecosystem

> **Document Type:** Design System & UI/UX Guidelines
> **Version:** 1.0
> **Last Revised:** 2026-08-06
> **Status:** Active
> **Audience:** UI/UX Designers, Frontend Developers, Mobile Developers

---

## Table of Contents
- [1. Design Philosophy](#1-design-philosophy)
- [2. Color Palette](#2-color-palette)
- [3. Typography](#3-typography)
- [4. Icons](#4-icons)
- [5. Grid System](#5-grid-system)
- [6. Components](#6-components)
- [7. Navigation](#7-navigation)
- [8. Dashboard Layouts](#8-dashboard-layouts)
- [9. Website Layouts](#9-website-layouts)
- [10. Mobile Layouts](#10-mobile-layouts)
- [11. Forms & Validation](#11-forms--validation)
- [12. Accessibility (a11y)](#12-accessibility-a11y)
- [13. Dark Mode Strategy](#13-dark-mode-strategy)
- [14. Micro Interactions](#14-micro-interactions)
- [15. UX Guidelines](#15-ux-guidelines)

---

## 1. Design Philosophy

MedicHp's interface must inspire **Trust**, ensure **Clarity**, and prioritize **Speed**. The UI should feel similar in quality to best-in-class platforms (e.g., Stripe, Notion, Apple Health, Linear) while maintaining a distinct, clinical, and approachable healthcare identity.

- **Minimalism:** Remove all non-essential elements. White space (negative space) is a primary design tool.
- **Accessibility:** Must be usable by elderly patients, visually impaired users, and those with motor difficulties.
- **Consistency:** The same visual language applies across the Next.js Website, React Dashboard, and React Native App.
- **Simplicity:** Actions should be obvious. Do not hide primary actions behind tooltips or complex menus.
- **Trust:** Healthcare is sensitive. High-quality UI, proper alignment, and robust error handling reassure users that their data is safe.

---

## 2. Color Palette

Our colors are grounded in healthcare psychology. Blues represent trust and calm, while greens represent health and success.

### 2.1 Core Colors (Light Mode)

| Category | Token Name | Hex Code | Usage | Psychology |
|----------|------------|----------|-------|------------|
| **Primary** | `primary-600` | `#026CB6` | Primary buttons, active tabs, main links | Trust, stability, professionalism |
| **Secondary**| `secondary-500`| `#10B981` | Accent elements, secondary actions | Health, growth, vitality |
| **Neutral** | `neutral-900` | `#111827` | Primary text, headings | Readability, grounding |
| **Neutral** | `neutral-500` | `#6B7280` | Secondary text, placeholders, icons | Hierarchy |
| **Surface** | `surface-50` | `#F9FAFB` | App background, large canvas areas | Cleanliness, clinical feel |
| **Surface** | `surface-100` | `#FFFFFF` | Cards, modals, dropdowns | Contrast and elevation |

### 2.2 Semantic Colors

| Intent | Token Name | Hex Code | Background Tint | Usage |
|--------|------------|----------|-----------------|-------|
| **Success** | `success-600` | `#059669` | `#D1FAE5` | Confirmed appointments, successful saves |
| **Warning** | `warning-500` | `#F59E0B` | `#FEF3C7` | Pending status, expiring tokens |
| **Danger** | `danger-600` | `#DC2626` | `#FEE2E2` | Errors, cancellations, destructive actions |
| **Info** | `info-500` | `#3B82F6` | `#DBEAFE` | Tooltips, informational banners |

### 2.3 Dark Mode Strategy
Dark mode uses deep slate and gray tones instead of pure black (`#000000`) to reduce eye strain.
- **Background:** `#0F172A` (Slate 900)
- **Surface:** `#1E293B` (Slate 800)
- **Primary Text:** `#F8FAFC` (Slate 50)
- **Primary Brand:** Softened to `#38BDF8` (Sky 400) for better contrast.

---

## 3. Typography

**Primary Font:** `Inter` (Sans-Serif) — Chosen for its exceptional legibility on digital screens, especially for numbers and dense data (test results, dosages).

### 3.1 Font Scale & Hierarchy

| Element | Size (rem) | Size (px) | Weight | Line Height | Usage |
|---------|------------|-----------|--------|-------------|-------|
| Display | 3.0rem | 48px | 700 (Bold) | 1.1 | Landing page hero |
| H1 | 2.25rem | 36px | 600 (Semibold)| 1.2 | Page titles (e.g., "Dashboard") |
| H2 | 1.875rem | 30px | 600 (Semibold)| 1.3 | Section headers |
| H3 | 1.5rem | 24px | 500 (Medium) | 1.4 | Card titles, modals |
| Body 1 | 1.0rem | 16px | 400 (Regular)| 1.5 | Standard paragraph text, inputs |
| Body 2 | 0.875rem | 14px | 400 (Regular)| 1.5 | Secondary text, table data |
| Caption | 0.75rem | 12px | 500 (Medium) | 1.5 | Badges, timestamps, helpers |

> **Rule:** Never use font sizes below `12px`. For primary reading areas, `16px` is the absolute minimum.

---

## 4. Icons

**Recommended Library:** [Lucide Icons](https://lucide.dev/) (consistent line weight, clean, modern).

- **Medical Icons:** Use standardized icons for heart rate (`Heart`), prescriptions (`Pill`), consultations (`Stethoscope`), and history (`ClipboardList`).
- **Navigation:** Keep navigation icons simple (`Home`, `Calendar`, `User`, `Settings`, `MessageSquare`).
- **Stroke Weight:** Consistent 2px stroke width.
- **Size:** 20x20px for inline text, 24x24px for navigation.

---

## 5. Grid System

MedicHp uses a standard 12-column grid system with fluid spacing.

### Breakpoints

| Device | Breakpoint (min-width) | Columns | Gutter | Margin |
|--------|------------------------|---------|--------|--------|
| Mobile | `320px` (xs) | 4 | 16px | 16px |
| Tablet | `768px` (md) | 8 | 24px | 24px |
| Desktop| `1024px` (lg)| 12 | 24px | 32px |
| Widescreen| `1440px` (2xl)| 12 | 32px | Auto (max-width 1280px) |

---

## 6. Components

All UI components must be atomic and reusable.

### 6.1 Buttons
- **Primary:** Solid background (`primary-600`), white text. Used for the single most important action on a screen (e.g., "Book Appointment").
- **Secondary:** Outline with `primary-600` border and text. Used for alternative actions.
- **Ghost/Tertiary:** No border, transparent background. Used for "Cancel" or "Go Back".
- **States:** Hover (darken 10%), Active/Pressed (darken 20%), Disabled (opacity 50%, unclickable).
- **Size:** Minimum touch target `44x44px` for mobile.

### 6.2 Healthcare-Specific Cards
- **Profile Card:** Avatar, Name, Specialization/Role, Rating, Quick Action button.
- **Appointment Card:** Date/Time (prominent), Doctor/Patient name, Status badge, "Reschedule" & "Cancel" ghost buttons.
- **Prescription Card:** Date issued, Doctor name, Medication list (truncated), "Download PDF" button.
- **Medical Timeline Card:** Vertical line connecting events (Consultation, Lab Result, Prescription) with timestamps.

### 6.3 Status Badges
Used heavily in healthcare for statuses.
- `Confirmed`: Green text, Light Green BG.
- `Pending`: Orange text, Light Orange BG.
- `Cancelled`: Red text, Light Red BG.
- `Completed`: Blue text, Light Blue BG.

---

## 7. Navigation

### 7.1 Web Application (Dashboard)
- **Sidebar:** Left-aligned, collapsible. Contains primary routes (Dashboard, Appointments, Patients/Doctors, Chat, Settings).
- **Top Header:** Global Search bar, Notification bell (with red dot for unread), User Profile dropdown.

### 7.2 Mobile Application
- **Bottom Navigation Bar:** Fixed at the bottom. 4-5 items max (Home, Appointments, Chat, Profile).
- **Floating Action Button (FAB):** Used only for the primary action (e.g., '+' to Book Appointment or Create Consultation).
- **Header:** Page title centered, Back button on the left, Contextual action on the right.

---

## 8. Dashboard Layouts

### 8.1 Patient Dashboard
- **Top:** "Hello, [Name]" + Actionable alert if profile is incomplete.
- **Hero Widget:** Next Upcoming Appointment (large card with countdown or prominent time).
- **Grid:** Quick action buttons (Find a Doctor, View Records).
- **List:** Recent Activity (last consultation, new prescription).

### 8.2 Doctor Dashboard
- **Top Metrics:** Today's Appointments, Pending Requests, Unread Messages.
- **Main View:** Daily Schedule (Timeline view showing booked slots and gaps).
- **Sidebar Widget:** Recent Patients list for quick access to history.

### 8.3 Super Admin Dashboard
- **Top Metrics:** Total Users, Revenue (Future), Active Doctors, Platform Health.
- **Charts:** User growth over time (Line chart), Appointments by status (Donut chart).
- **Data Table:** Recent registrations requiring audit or flagged accounts.

---

## 9. Website Layouts (Public)

- **Landing Page:** High-trust hero section, clear value proposition, "Search Doctors" prominent input, social proof (reviews/stats).
- **Doctor Search:** Left sidebar for filters (Specialty, City, Fee). Right side for scrollable list of Doctor Cards.
- **Doctor Profile:** Two-column layout on desktop. Left: Photo, Bio, Details. Right: Sticky booking widget with calendar slots.

---

## 10. Forms & Validation

Healthcare forms are notoriously long. MedicHp solves this through:
- **Progressive Disclosure:** Break long forms (like patient registration) into multi-step wizards.
- **Input Fields:** Clearly labeled (labels *above* inputs). Helper text below inputs.
- **Validation:** 
  - Inline real-time validation (show green checkmark when valid).
  - Clear error messages (e.g., "Password must contain a number", NOT "Invalid input").
  - Auto-formatting (e.g., phone numbers format automatically to `+92 300 1234567`).
- **Required Fields:** Explicitly marked with an asterisk `*`.

---

## 11. Accessibility (a11y)

MedicHp strictly adheres to **WCAG 2.1 AA** standards.

- **Contrast:** Text-to-background contrast ratio must be at least `4.5:1` for normal text and `3:1` for large text.
- **Keyboard Navigation:** All interactive elements must be focusable via `Tab`. Use a highly visible focus ring (blue outline).
- **Screen Readers:** Use semantic HTML (`<nav>`, `<main>`, `<article>`). Ensure `aria-labels` are present on icon-only buttons.
- **Click Areas:** Minimum interactive target size is `44x44px` on touch devices to accommodate users with poor motor control.

---

## 12. Dark Mode Strategy

- Dark mode is toggleable via user settings and respects the system default (`prefers-color-scheme`).
- Do not invert colors directly. Use the defined semantic dark palette to ensure contrast is maintained without being harsh on the eyes.
- Shadows in dark mode are replaced by subtle borders (`1px solid #334155`) to separate overlapping surfaces.

---

## 13. Micro Interactions

Micro-interactions provide feedback and create a premium feel.

- **Loading:** Use Skeleton loaders mimicking the layout instead of generic spinners for full-page loads. Use small spinners inside buttons when submitting forms.
- **Transitions:** Max `200ms` ease-in-out for hover states and modal openings.
- **Success States:** Brief snackbar at the bottom/top of the screen. Example: "✅ Appointment confirmed."
- **Empty States:** When a list is empty (e.g., No Appointments), show a friendly illustration, a brief explanation, and a Call to Action (CTA) button to create one.

---

## 14. UX Guidelines

1. **Rule of One:** Each screen should have ONE primary objective.
2. **Minimize Cognitive Load:** Don't overwhelm users with data. Use tabs, accordions, and pagination to organize heavy medical records.
3. **Forgiving UI:** Actions like "Cancel Appointment" should prompt a confirmation dialog ("Are you sure?"). Actions like "Delete" (where permitted) should require typing a confirmation word.
4. **Predictability:** The "Save" button should always be in the same place. Navigation should never jump around.
5. **No Dead Ends:** If a search yields no results, suggest an alternative ("No cardiologists found in Lahore. View cardiologists in nearby cities?").
