import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';

import { ToastrService } from 'ngx-toastr';

import { LoginUser } from './../models/login-user';
import { ContaService } from './../services/conta.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {

    loginForm: FormGroup;
    usuario: LoginUser;

    constructor(private fb: FormBuilder,
                private contaService: ContaService,
                private router: Router,
                private toastr: ToastrService)
    { }

    public ngOnInit(): void {
        this.loginForm = this.fb.group({
            email: [''],
            password: ['']
        });
    }

    public login(): void {
        this.usuario = Object.assign({}, this.usuario, this.loginForm.value);

        this.contaService.login(this.usuario)
            .subscribe(
                sucesso => { this.processarSucesso(sucesso); },
                falha => { this.processarFalha(falha); }
            );
    }

    public processarSucesso(response: any): void {
        this.loginForm.reset();

        this.contaService.LocalStorage.salvarDadosLocaisUsuario(response);
        this.router.navigate(['/home']).then(() => { this.toastr.success('Login efetuado com sucesso!'); });
    }

    public processarFalha(response: any): void {
        this.toastr.error(response.error.Error);
    }
}
