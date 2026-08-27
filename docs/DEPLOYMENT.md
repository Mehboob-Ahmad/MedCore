# MedicHp Deployment Guide

This document outlines the procedures for deploying the MedicHp Phase 1 application to a production environment on DigitalOcean.

## Architecture

The system consists of:
- **Web**: Next.js application running as a standalone node server.
- **API**: ASP.NET Core 9 application running on Kestrel.
- **Database**: PostgreSQL 15 for relational data.
- **Cache**: Redis 7 for distributed caching and rate limiting.

All services are containerized using Docker and orchestrated using Docker Compose.

## Prerequisites
1. A DigitalOcean Droplet (Ubuntu 22.04 LTS recommended) with at least 4GB RAM and 2 vCPUs.
2. Docker and Docker Compose installed on the Droplet.
3. Domain names pointed to the Droplet's IP address (e.g., `app.medichp.com`, `api.medichp.com`).
4. SSH access to the Droplet.

## Deployment Steps

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/your-org/MediCore.git /opt/medichp
   cd /opt/medichp
   ```

2. **Configure Environment Variables**:
   Create a `.env` file in the root directory:
   ```env
   DB_PASSWORD=your_secure_db_password
   JWT_SECRET=your_secure_jwt_secret_at_least_32_characters_long
   ```

3. **Start the Services**:
   ```bash
   docker-compose -f docker-compose.prod.yml up -d --build
   ```

4. **SSL / Reverse Proxy Configuration**:
   It is recommended to place an Nginx reverse proxy or DigitalOcean Load Balancer in front of the services to handle SSL termination. Configure the proxy to forward traffic for `app.medichp.com` to port `3000` and `api.medichp.com` to port `5000`.

## Backup and Recovery Strategy

### Database Backups
Automated logical backups should be configured using `pg_dump` via a cron job on the host machine.

**Backup Script (`/opt/backups/backup.sh`)**:
```bash
#!/bin/bash
BACKUP_DIR="/opt/backups/db"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
docker exec medicore_db_1 pg_dump -U postgres medichp_prod > $BACKUP_DIR/medichp_prod_$TIMESTAMP.sql
gzip $BACKUP_DIR/medichp_prod_$TIMESTAMP.sql
```

**Cron Configuration**:
Run daily at 2 AM:
`0 2 * * * /opt/backups/backup.sh`

### Restoration Procedure
To restore from a backup:
```bash
gunzip -c /opt/backups/db/medichp_prod_timestamp.sql.gz | docker exec -i medicore_db_1 psql -U postgres -d medichp_prod
```

### File Backups
Any uploaded files (if stored locally instead of S3) should be synchronized using `rsync` or `rclone` to an offsite location like DigitalOcean Spaces.

## CI/CD Pipeline
The repository includes a GitHub Actions workflow (`.github/workflows/ci-cd.yml`) that automatically builds, tests, and deploys the application to the DigitalOcean Droplet upon a push to the `main` branch. 
Required GitHub Secrets:
- `DO_HOST`: Droplet IP address
- `DO_USERNAME`: SSH username (e.g., `root`)
- `DO_SSH_KEY`: SSH private key for access
