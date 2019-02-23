# Argo Manager

An API that simplefies argo tunnel management.

## Motivation

It is kind of painful to install `cloudflared` on every machine and managing the tunnel.
I was looking for a tool to automate this process but it seems that there is no tool that fits my usecase.

So i wrote one 😁

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

## Future Ideas

* [ ] Add Swagger
* [ ] Tunnel Health Checking
* [ ] Expose Tunnel logger
* [ ] User Interface