# Multi-stage Dockerfile for BullMarket AI Store Frontend
# Compatible with Podman and Docker

# Stage 1: Build static assets
FROM node:22-alpine AS build
WORKDIR /app

# Install dependencies first for optimal layer caching
COPY package.json package-lock.json ./
RUN npm ci

# Copy application source code
COPY . .

# Build arguments for Vite environment
ARG VITE_API_BASE_URL="http://localhost:5164"
ARG VITE_CUSTOMER_ID="11111111-1111-1111-1111-111111111111"

ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
ENV VITE_CUSTOMER_ID=$VITE_CUSTOMER_ID

# Build production bundle
RUN npm run build

# Stage 2: Serve with Nginx
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Clean default nginx files
RUN rm -rf ./*

# Copy built frontend assets
COPY --from=build /app/dist .

# Copy custom Nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
