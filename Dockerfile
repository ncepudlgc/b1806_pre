FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim

# Install basic development tools
RUN apt-get update && apt-get install -y \
    git \
    curl \
    wget \
    vim \
    nano \
    && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /workspace

# Copy project files
COPY . .

# Restore dependencies (but don't build the application)
WORKDIR /workspace/src/SonyBraviaControl
RUN dotnet restore

# Return to workspace root
WORKDIR /workspace

# Default to bash shell for development
CMD ["/bin/bash"]