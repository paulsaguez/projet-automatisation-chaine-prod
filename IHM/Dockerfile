# Étape 1: Build de l'application Angular
FROM node:20-alpine AS build

WORKDIR /app

# Installation des dépendances
COPY package*.json ./
RUN npm ci

# Copie du code source et build
COPY . .
RUN npm run build --configuration=production

# Étape 2: Serveur Nginx pour servir l'application
FROM nginx:alpine

# Copie de la configuration Nginx personnalisée
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copie des fichiers buildés depuis l'étape précédente
COPY --from=build /app/dist/ihm/browser /usr/share/nginx/html

# Expose le port 80
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]