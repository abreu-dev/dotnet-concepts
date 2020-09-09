import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { LocalStorageUtils } from 'src/app/utils/localstorage';

@Component({
  selector: 'app-menu-login',
  templateUrl: './menu-login.component.html'
})
export class MenuLoginComponent {
  public token = '';
  public user: any;
  public email = '';
  public LocalStorage = new LocalStorageUtils();

  constructor(private router: Router) {  }

  public usuarioLogado(): boolean {
    this.token = this.LocalStorage.obterToken();
    this.user = this.LocalStorage.obterUsuario();

    if (this.user) {
      this.email = this.user.email;
    }

    return this.token !== null;
  }

  public logout(): void {
    this.LocalStorage.limparDadosLocaisUsuario();
    this.router.navigate(['/conta/login']);
  }
}
