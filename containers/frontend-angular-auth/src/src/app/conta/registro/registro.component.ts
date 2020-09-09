import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';

import { ToastrService } from 'ngx-toastr';

import { RegisterUser } from './../models/register-user';
import { ContaService } from './../services/conta.service';

@Component({
    selector: 'app-registro',
    templateUrl: './registro.component.html'
})
export class RegistroComponent implements OnInit {

    registroForm: FormGroup;
    usuario: RegisterUser;

    constructor(private fb: FormBuilder,
                private contaService: ContaService,
                private router: Router,
                private toastr: ToastrService)
    { }

    public ngOnInit(): void {
        this.registroForm = this.fb.group({
            email: [''],
            password: ['']
        });
    }

    public registrar(): void {
        this.usuario = Object.assign({}, this.usuario, this.registroForm.value);

        this.contaService.registrar(this.usuario)
            .subscribe(
                sucesso => { this.processarSucesso(sucesso); },
                falha => { this.processarFalha(falha); }
            );
    }

    public processarSucesso(response: any): void {
        this.registroForm.reset();

        this.contaService.LocalStorage.salvarDadosLocaisUsuario(response);
        this.router.navigate(['/home']).then(() => { this.toastr.success('Conta criada com sucesso!'); });
    }

    public processarFalha(response: any): void {
        response.error.errors.forEach(error => {
            this.toastr.error(error);
        });
    }
}
