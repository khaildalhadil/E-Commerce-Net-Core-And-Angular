import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AccountService } from '../../services/AccountService';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const accountService = inject(AccountService);
  const token = accountService.currentUser()?.token;

  if (!token) {
    return next(req);
  }

  return next(req.clone({
    setHeaders: { Authorization: `Bearer ${token}` }
  }));
};
