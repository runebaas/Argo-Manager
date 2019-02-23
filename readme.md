# Argo Manager

An API that simplefies argo tunnel management.

## Motivation

Stuff...

## How it works

It spins up a docker container with `cloudflared`

## How to use it

### Local

#### Requirements

* DotNet Core 2.2 or higher
* Docker
* Active Argo Subscription

### In Docker

`docker run -d -p 5001:8080 -v /var/run/docker.sock:/var/run/docker.sock -v /path/to/cloudflared/cert.pem:/data/cert.pem runebaas/argo-manager`