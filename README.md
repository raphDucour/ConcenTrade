# ConcenTrade

**Branche finale : `Version-Finale-2`**

**ConcenTrade** est une application Windows de productivité combinant un **système Pomodoro** et un **mécanisme de récompenses**.  
Elle aide l’utilisateur à rester concentré en **bloquant automatiquement les applications distrayantes** pendant les sessions de travail, tout en proposant une progression ludique (cartes, coffres, points).

> **Note :** L'application nécessite les **droits d'administrateur** pour fonctionner correctement (interaction avec les processus Windows et WMI).

---

## Répartition de l’équipe

Projet réalisé à cinq, avec une répartition claire des responsabilités :

- **Antoine Dupuy** : Backend, surveillance système Windows (WMI), gestion des processus actifs et configuration des règles de blocage.
- **Raphaël Ducournau** : paramètres globaux, système d'authentification/session, logique de collection de cartes et Architecture de données (Supabase).
- **Leonardo Dib** : Moteur Pomodoro (timers, cycles travail/pause, calcul des points) / Outillage et workflow / Optimisation du développement.
- **Oscar Brochard** : UI/UX (WPF), animations et logique visuelle de la boutique.
- **Volcy Desmazures** : Navigation, et tutoriel interactif, base de données.

---

## Détails techniques – Antoine Dupuy

J'ai implémenté :

- **Moteur de surveillance (`AppBlocker.cs`)** : Détection événementielle via **WMI** (`Win32_ProcessStartTrace`) pour intercepter les processus en temps réel sans surcharger le CPU, avec gestion des arbres de processus liés (ex. Steam et ses sous-services).
- **Analyse d’état initial (`DistractingAppsConfirmation.xaml.cs`)** : Scan au lancement des processus actifs (`Process.GetProcesses`), filtrage via liste noire et proposition de fermeture groupée intelligente.
- **Gestion des règles (`BlockedAppsSettings.xaml.cs`)** : Configuration dynamique basée sur des collections observables et des `HashSet` pour une gestion efficace des applications ciblées.


## Détails techniques – Raphaël Ducournau

J'ai conçu l'infrastructure de données et le système de progression du projet :

- **Authentification et Persistance de Session**

- **Système de Collection et Gamification**

- **Stack Technique :** Supabase (BaaS), Properties.Settings & Global Variables

## Détails techniques – Leonardo Dib

- **Moteur Logique : Développement complet du système de cycles Pomodoro (travail / pause) et gestion des timers.**
- **Outillage et Workflow : Création de fonctionnalités d'administration internes pour faciliter les phases de test et la gestion des données de session.**
- **Optimisation du développement : Automatisation de la réinitialisation des environnements de test pour fluidifier le travail de l'équipe.**
