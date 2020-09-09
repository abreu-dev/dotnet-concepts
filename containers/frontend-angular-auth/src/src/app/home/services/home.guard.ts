import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

import { LocalStorageUtils } from 'src/app/utils/localstorage';

@Injectable()
export class HomeGuard implements CanActivate {

    public LocalStorage = new LocalStorageUtils();

    constructor(private router: Router) {}

    public canActivate(): boolean {

        if (!this.LocalStorage.obterToken()) {
            this.router.navigate(['/conta/login']);
        }

        return true;
    }
}
