@echo off
mkdir ssh-key
ssh-keygen -m PEM -t rsa -b 4096 -f "ssh-key/ssh-key"
