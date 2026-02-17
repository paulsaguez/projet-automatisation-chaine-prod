# Projet Automatisation de la Chaîne de Production

Ce projet met en œuvre une architecture microservices pour automatiser l'ingestion, le traitement et la visualisation de rapports de migration (Google Drive). Il a été conçu dans le cadre du module "Automatisation de la chaîne de programmation".

## 🚀 Fonctionnalités

- **Ingestion asynchrone** : Traitement des fichiers CSV lourds en arrière-plan via FastAPI.
- **Recherche avancée** : Filtrage multicritères (Titre, Source, Destination, Statut, etc.) sur les rapports migrés.
- **Architecture microservices** : Séparation claire des responsabilités (Frontend, Backend, Traitement, Base de données).
- **Conteneurisation complète** : Déploiement simplifié via Docker Compose.
- **Point d'entrée unique** : Reverse proxy Nginx exposant l'application sur un seul port (8080).

## 🛠 Architecture Technique

Le projet repose sur 4 services principaux orchestrés via Docker :

| Service        | Technologie      | Rôle                                 | Port Interne      |
| -------------- | ---------------- | ------------------------------------ | ----------------- |
| **IHM**        | Angular 16+      | Interface utilisateur responsive     | 8080              |
| **API**        | .NET 8 (C#)      | Gestion des données et règles métier | 5000              |
| **TRAITEMENT** | Python (FastAPI) | Parsing CSV et ingestion asynchrone  | 8000              |
| **BDD**        | MongoDB          | Stockage NoSQL des rapports          | 27017             |
| **Proxy**      | Nginx            | Reverse proxy et routage             | **8080 (Public)** |

## 📋 Prérequis

- **Docker** et **Docker Compose** installés sur la machine.
- Le fichier de données source `GDriveMigrationReport_...csv` doit être présent à la racine du projet (ou configuré dans le volume partagé).

## 🔧 Installation et Démarrage

1. **Cloner le dépôt** :

   ```bash
   git clone <url-du-repo>
   cd projet-automatisation-chaine-prod
   ```

2. **Démarrer l'application** :

   ```bash
   docker-compose up --build -d
   ```

   _L'option `--build` assure que les images sont reconstruites, et `-d` lance les conteneurs en arrière-plan._

3. **Accéder à l'application** :
   - Ouvrez votre navigateur sur : [http://localhost:8080](http://localhost:8080)

## 📂 Structure du Projet

```
.
├── API/              # Backend .NET (Controllers, Models, Services)
├── IHM/              # Frontend Angular (Pages, Composants)
├── TRAITEMENT/       # Service Python (FastAPI, Pandas)
├── nginx/            # Configuration du Reverse Proxy
├── docker-compose.yml # Orchestration des conteneurs
└── README.md         # Documentation du projet
```

## 🔍 Utilisation

1. **Page d'accueil** : Vue d'ensemble ou upload de nouveaux fichiers (selon implémentation).
2. **Recherche** :
   - Naviguez vers la page de recherche.
   - Utilisez la barre de recherche globale ou les filtres spécifiques (Statut, Source/Destination, etc.).
   - Les résultats s'affichent dynamiquement sous forme de tableau.

## 👥 Auteurs

- **Théo TORNATORE**
- **Paul SAGUEZ**
  _(Groupe BUT3-ALT)_
