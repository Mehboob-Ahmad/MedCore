export const env = {
  NEXT_PUBLIC_API_URL: process.env.NEXT_PUBLIC_API_URL,
};

// Validate environment variables on startup
if (typeof window === 'undefined') {
  if (!env.NEXT_PUBLIC_API_URL) {
    throw new Error('Missing environment variable: NEXT_PUBLIC_API_URL');
  }
}
