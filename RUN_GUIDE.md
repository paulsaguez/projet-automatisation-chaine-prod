# Guide de Démarrage - Projet Automatisation

Ce document explique comment lancer et utiliser le projet.

## Prérequis

- Docker et Docker Compose installés.

## Lancement

1. **Cloner le dépôt** (si ce n'est pas déjà fait).
2. **Démarrer les services** :

   ```bash
   docker-compose up --build
   ```

   _Note : L'option `--build` est recommandée lors du premier lancement ou après modification du code pour reconstruire les images._

3. **Accéder à l'application** :
   - Ouvrez votre navigateur sur `http://localhost:8080`.
   - Le port 8080 est le seul point d'entrée exposé (via Nginx).

## Utilisation

1. **Upload de fichier** :
   - Sur la page d'accueil, cliquez sur "Ajouter des données" (ou naviguez via le bouton upload).
   - Sélectionnez un fichier CSV (format attendu : logs de migration Google Drive).
   - Cliquez sur "Envoyer".

2. **Traitement** :
   - Le fichier est envoyé par morceaux au service Python.
   - Le service Python reconstitue le fichier, le lit, et calcule un hash unique pour chaque ligne.
   - Il interroge l'API C# pour vérifier si ces données existent déjà.
   - Seules les nouvelles données sont insérées en base.
   - Un message de confirmation vous indiquera combien de lignes ont été traitées et combien ont été insérées.

3. **Consultation** :
   - Utilisez la recherche pour visualiser les données importées.

## Architecture & Sécurité

- **Nginx (Port 8080)** : Seul point d'entrée public. Redirige vers :
  - `/` -> Frontend Angular
  - `/traitement/` -> Service Python
  - `/api/` -> API C#
- **API C#** : Gère la logique métier et l'accès MongoDB. Ne peut être contactée directement de l'extérieur (sauf via Nginx).
- **Traitement Python** : Gère le parsing et l'ingestion. Interne au réseau Docker.
- **MongoDB** : Stockage des données. Interne au réseau Docker.
