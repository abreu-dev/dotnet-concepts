export class LocalStorageUtils {

    public salvarDadosLocaisUsuario(response: any): void {
        localStorage.setItem('mist.token', response.accessToken);
        localStorage.setItem('mist.user', JSON.stringify(response.userToken));
    }

    public limparDadosLocaisUsuario(): void {
        localStorage.removeItem('mist.token');
        localStorage.removeItem('mist.user');
    }

    public obterUsuario(): any {
        return JSON.parse(localStorage.getItem('mist.user'));
    }

    public obterToken(): any {
        return localStorage.getItem('mist.token');
    }
}
