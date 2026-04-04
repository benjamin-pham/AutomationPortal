import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Required for Docker: produces a self-contained server in .next/standalone
  output: "standalone",
  allowedDevOrigins: ["172.26.0.14"],
};

export default nextConfig;
