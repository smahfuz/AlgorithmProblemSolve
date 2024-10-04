#include<bits/stdc++.h>
using namespace std;

void sieve(int n)
{
    int kk=1;
    int arr[n+5]={0};
    for(int i=2; i*i<=n; i++){

        if(arr[i]==0){

            for(int j=i*i; j<=n; j+=i){
            arr[j]=1;
           }

        }

    }

    for(int i=2; i<=n; i++){
        if(arr[i]==0) {

            cout<<"number "<<kk<<" prime number is :" <<i<<endl;
            kk++;
        }
    }
}

int main()
{
    int x; cin>>x;
    sieve(x);
    return 0;
}
