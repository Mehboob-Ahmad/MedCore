# 📱 Apps

This directory contains all frontend applications that make up the MedicHp platform.

## Structure

```
apps/
├── website/      → Public-facing marketing & informational site (Next.js)
├── dashboard/    → Admin & clinical management dashboard (React + Vite)
└── mobile/       → Patient & doctor mobile application (React Native + Expo)
```

## Architecture Rationale

Each application is independently deployable while sharing common packages from `packages/`. This separation ensures:

- **Independent scaling** — Each app can be deployed and scaled on its own infrastructure
- **Team autonomy** — Separate teams can work on different apps without merge conflicts
- **Technology flexibility** — Each app uses the framework best suited for its platform
- **Optimized bundles** — No cross-app code bloat; shared code is pulled from `packages/`

## Applications

### `website/`
Public-facing Next.js application serving as the primary marketing and informational presence. Uses Server-Side Rendering (SSR) for SEO optimization and fast initial load times.

### `dashboard/`
Internal React + Vite application for clinic administrators, doctors, and super admins. Provides real-time analytics, patient management, appointment scheduling, and system configuration.

### `mobile/`
Cross-platform React Native + Expo application for patients and doctors. Offers appointment booking, telemedicine, health records access, and push notifications.
