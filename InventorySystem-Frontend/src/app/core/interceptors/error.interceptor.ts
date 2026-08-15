import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { ToastService } from '../services/toast.service';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toastService = inject(ToastService);
  const authService = inject(AuthService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error) => {
      if (error.status === 401) {
        // If the request was for login, it's just invalid credentials
        if (req.url.includes('/login')) {
          toastService.error('Credenciales inválidas. Verifica tu usuario y contraseña.');
        } else {
          toastService.error('Sesión expirada. Por favor, inicie sesión nuevamente.');
          authService.logout();
          router.navigate(['/login']);
        }
      } else if (error.status === 400) {
        let message = 'Petición inválida';
        if (error.error?.error?.description) {
          message = error.error.error.description;
        } else if (error.error?.error?.Description) {
          message = error.error.error.Description;
        } else if (error.error?.error?.message) {
          message = error.error.error.message;
        } else if (error.error?.Error?.Description) {
          message = error.error.Error.Description;
        } else if (error.error?.title) {
          message = error.error.title;
          if (error.error.errors) {
            const validationErrs = Object.values(error.error.errors).flat().join(' ');
            message = validationErrs; // Override with the specific validation messages
          }
        } else if (typeof error.error === 'string') {
          message = error.error;
        } else if (typeof error.error?.error === 'string') {
          message = error.error.error;
        }
        
        console.error('API Error:', error.error);
        toastService.error(message);
      } else if (error.status === 404) {
        toastService.error('Recurso no encontrado.');
      } else {
        toastService.error('Ocurrió un error inesperado en el servidor.');
      }
      return throwError(() => error);
    })
  );
};
