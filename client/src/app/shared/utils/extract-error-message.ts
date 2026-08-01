export function extractErrorMessage(err: any): string {
  if (typeof err?.error === 'string') return err.error;
  if (Array.isArray(err?.error)) return err.error.join(', ');
  return err?.error?.message ?? 'Something went wrong. Please try again.';
}
