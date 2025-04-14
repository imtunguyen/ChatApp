import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { HttpResponse } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  get<T>(endpoint: string): Observable<T> {
    return this.http.get<T>(`${this.apiUrl}${endpoint}`).pipe(
      catchError(this.handleError.bind(this)) // 👉 bind để giữ context `this`
    );
  }
  

  post<T>(endpoint: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.apiUrl}${endpoint}`, body).pipe(
      catchError(this.handleError)
    );
  }

  put<T>(endpoint: string, body: T): Observable<T> {
    return this.http.put<T>(`${this.apiUrl}${endpoint}`, body).pipe(
      catchError(this.handleError)
    );
  }

  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(`${this.apiUrl}${endpoint}`).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    // ❌ Không log lỗi nếu là 404
    if (error.status !== 404) {
      console.error('API error:', error);
    }
  
    let errorMessage = 'Đã xảy ra lỗi. Vui lòng thử lại sau.';
  
    if (typeof error.error === 'string') {
      errorMessage = error.error;
    } else if (error.error?.message) {
      errorMessage = error.error.message;
    } else if (Array.isArray(error.error) && error.error[0]?.description) {
      errorMessage = error.error.map((err: any) => err.description).join('\n');
    }
  
    return throwError(() => new Error(errorMessage));
  }
  
  
}
