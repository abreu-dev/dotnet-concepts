import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { LocalStorageUtils } from './../../utils/localstorage';
import { LoginUser } from './../models/login-user';
import { RegisterUser } from './../models/register-user';

@Injectable()
export class ContaService {

    public LocalStorage = new LocalStorageUtils();

    constructor(private http: HttpClient) { }

    public login(usuario: LoginUser): Observable<LoginUser> {
        return this.http
            .post('https://localhost:8888/api/v1/' + 'login', usuario, this.obterHeaderJson())
            .pipe(
                map(this.extractData),
                catchError(this.serviceError));
    }

    public registrar(usuario: RegisterUser): Observable<RegisterUser> {
        return this.http
            .post('https://localhost:8888/api/v1/' + 'register', usuario, this.obterHeaderJson())
            .pipe(
                map(this.extractData),
                catchError(this.serviceError));
    }

    public extractData(response: any): any {
        return response.data || {};
    }

    public serviceError(response: Response | any): Observable<never> {
        if (response instanceof HttpErrorResponse) {

            if (response.statusText === 'Unknown Error') {
                response.error.Error = 'An unexpected error has occurred.';
            }
        }

        console.error(response);
        return throwError(response);
    }

    protected obterHeaderJson(): any {
        return {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        };
    }
}
