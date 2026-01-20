#!/bin/bash

# Example script to add a device to SterileTrack API

# Example 1: Basic device
curl -X 'POST' \
  'http://localhost:5002/api/devices' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "deviceIdentifier": "DEV-001",
  "name": "Surgical Scissors",
  "description": "Standard surgical scissors",
  "manufacturer": "MedTech Inc",
  "modelNumber": "SC-2024",
  "isReusable": true
}'

# Example 2: Disposable device
# curl -X 'POST' \
#   'http://localhost:5002/api/devices' \
#   -H 'accept: application/json' \
#   -H 'Content-Type: application/json' \
#   -d '{
#   "deviceIdentifier": "DEV-002",
#   "name": "Disposable Scalpel",
#   "description": "Single-use scalpel",
#   "manufacturer": "Surgical Supply Co",
#   "modelNumber": "DS-100",
#   "isReusable": false
# }'
