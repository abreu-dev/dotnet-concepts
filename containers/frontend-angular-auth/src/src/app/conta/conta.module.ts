import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { ContaAppComponent } from './conta.app.component';
import { ContaRoutingModule } from './conta.route';
import { LoginComponent } from './login/login.component';
import { RegistroComponent } from './registro/registro.component';
import { ContaService } from './services/conta.service';

@NgModule({
    declarations: [
        ContaAppComponent,
        LoginComponent,
        RegistroComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
        HttpClientModule,
        ContaRoutingModule
    ],
    providers: [
        ContaService
    ]
})
export class ContaModule { }
