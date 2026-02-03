# start wsl
```
wsl --list --verbose
wsl -d Ubuntu-22.04
```

# build docker
```
run build_and_run.bat
```

# Run app
```
dos2unix docker_task_api.sh
chmod +x docker_task_api.sh
./docker_task_api.sh
```

# Config nginx
```
server {
    server_name local.task.test.com;

    client_max_body_size 6000M;

    location / {
        proxy_pass http://127.0.0.1:3589;

        proxy_http_version 1.1;

        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";

        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;

        proxy_read_timeout 86400;
        proxy_send_timeout 86400;
    }
}
```