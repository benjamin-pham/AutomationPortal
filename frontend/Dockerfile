# syntax=docker/dockerfile:1

# ── Stage 1: Prod dependencies (runtime only) ─────────────────────────────────
FROM node:22-alpine AS deps-prod
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci --omit=dev

# ── Stage 2: Builder (all deps including devDependencies for build tools) ─────
FROM node:22-alpine AS builder
WORKDIR /app

# Receive backend URL at build time (used by Next.js server-side at runtime too)
ARG API_URL=http://backend:5000
ENV API_URL=$API_URL

# Install ALL dependencies (including devDeps like lightningcss, tailwind, etc.)
# so native binaries are compiled for linux-x64-musl (Alpine)
COPY package.json package-lock.json ./
RUN npm ci

COPY . .

# Disable Next.js telemetry
ENV NEXT_TELEMETRY_DISABLED=1

RUN npm run build

# ── Stage 3: Runner ───────────────────────────────────────────────────────────
FROM node:22-alpine AS runner
WORKDIR /app

ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1

# Create non-root user
RUN addgroup --system --gid 1001 nodejs \
 && adduser  --system --uid 1001 nextjs

# Copy only what Next.js needs at runtime
COPY --from=builder /app/public              ./public
COPY --from=builder --chown=nextjs:nodejs /app/.next/standalone  ./
COPY --from=builder --chown=nextjs:nodejs /app/.next/static      ./.next/static

USER nextjs

EXPOSE 3000

ENV HOSTNAME="0.0.0.0"
ENV PORT=3000

CMD ["node", "server.js"]
