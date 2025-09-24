# Multi-stage Dockerfile for Angular application

# Development stage
FROM node:24-alpine AS development
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies (use install to allow lock file regeneration)
RUN npm install

# Copy source code
COPY . .

# Expose port for development server
EXPOSE 4200

# Start development server with hot reload
CMD ["npm", "start", "--", "--host", "0.0.0.0", "--port", "4200", "--poll", "2000", "--live-reload", "true", "--watch", "true", "--disable-host-check"]

# Build stage
FROM node:24-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies (use install to allow lock file regeneration)
RUN npm install

# Copy source code
COPY . .

# Build the application
RUN npm run build

# Production stage
FROM nginx:alpine AS production

# Copy custom nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy built application
COPY --from=build /app/dist/house_hub_fea /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]