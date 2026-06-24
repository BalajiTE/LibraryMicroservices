import { HttpErrorResponse } from '@angular/common/http';

export function formatHttpError(error: unknown, fallback: string): string {
  if (!(error instanceof HttpErrorResponse)) {
    return fallback;
  }

  if (error.status === 401) {
    return 'Session expired or not signed in. Please sign in again.';
  }

  if (error.status === 403) {
    return 'You do not have permission for this action. Try signing in as librarian or admin.';
  }

  if (error.status === 0) {
    return 'Cannot reach the API. Ensure the gateway and services are running on port 5000.';
  }

  const body = error.error as { error?: string; detail?: string; title?: string } | null;
  return body?.error ?? body?.detail ?? body?.title ?? fallback;
}
